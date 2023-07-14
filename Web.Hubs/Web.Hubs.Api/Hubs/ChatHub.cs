using Web.Hubs.Core.Services;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Messages;
using Web.Hubs.Core.Dtos.UserChat;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Web.Hubs.Core.Dtos;

namespace Web.Hubs.Api.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IStorage<long> connections;
    private readonly ICallService callService;
    private readonly IChatPresenter chatPresenter;
    private readonly IMessageService messageService;
    private readonly IChatUserService chatUserService;

    public ChatHub(
        IStorage<long> storage,
        ICallService callService,
        IChatPresenter chatPresenter,
        IMessageService messageService,
        IChatUserService userChatService)
    {
        this.connections = storage;
        this.callService = callService;
        this.chatPresenter = chatPresenter;
        this.messageService = messageService;
        this.chatUserService = userChatService;
    }

    public async Task StartCall(Guid chatId)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await callService.Create(chatId, userId);

        await result.Match(
            success => NotifyUsers(chatId, new { chatId, userId }, nameof(StartCall)),
            chatNotFound => Task.CompletedTask, //NotifyUser(userId, "Chat not found", "Error"))
            callAlreadyExists => Task.CompletedTask //NotifyUser(userId, "Call already exists", "Error"))
        );
    }

    public async Task JoinCall(JoinCall join)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await callService.Add(join.ChatId, userId);

        await result.Match(
            success => NotifyUsers(join.ChatId, join, nameof(JoinCall)),
            userInCall => Task.CompletedTask, // leave call?
            callNotFound => Task.CompletedTask //NotifyUser(userId, "Call not found", "Error"))
        );
    }

    public async Task LeaveCall(LeaveCall leave)
    {
        var userId = Context.User.GetUserId<long>();

        await callService.Delete(leave.ChatId, userId);

        await NotifyUsers(leave.ChatId, leave, nameof(LeaveCall));
    }

    public async Task EndCall(Guid chatId)
    {
        var userId = Context.User.GetUserId<long>();

        await callService.Delete(chatId);

        await NotifyUsers(chatId, chatId, nameof(EndCall));
    }

    public async Task SendMessage(CreateMessageDto message)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.Create(message, userId);

        await result.Match(
            message => NotifyUsers(message.ChatId, message, nameof(SendMessage)),
            error => Task.CompletedTask  //NotifyUser(userId, "Call not found", "Error"))
        );
    }

    public async Task UpdateChat(UpdateChatUserDto userChat)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await chatUserService.Update(userChat.ChatId, userId, userChat.LastRead);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(userChat.ChatId, userChat.ChatId, nameof(UpdateChat));
    }

    public Task ChatCreated(Guid chatId) => NotifyUsers(chatId, chatId, nameof(ChatCreated));

    private async Task NotifyUsers<TMessage>(Guid chatId, TMessage? message, string method)
    {
        var users = await chatPresenter.GetUsers(chatId);

        if (users?.Length > 0)
        {
            var connections = await this.connections.Get(users);

            await Clients.Clients(connections)
                .SendAsync(method, message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.GetUserId<long>();

        await connections.Add(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User.GetUserId<long>();

        await connections.Delete(userId, Context.ConnectionId);

        var callId = await callService.Delete(userId);
        if (callId != default)
        {
            await NotifyUsers(callId, callId, nameof(LeaveCall));
        }

        await base.OnDisconnectedAsync(exception);
    }
}