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
    private readonly IStorage<long> userConnections;
    private readonly ICallService callService;
    private readonly IChatPresenter chatPresenter;
    private readonly IMessageService messageService;
    private readonly IUserChatService userChatService;

    public ChatHub(
        IStorage<long> storage,
        ICallService callService,
        IChatPresenter chatPresenter,
        IMessageService messageService,
        IUserChatService userChatService)
    {
        this.userConnections = storage;
        this.callService = callService;
        this.chatPresenter = chatPresenter;
        this.messageService = messageService;
        this.userChatService = userChatService;
    }

    public Task<CallDto> GetCall(Guid chatId)
    {
        return callService.Get(chatId);
    }

    public async Task Calling(StartCall call)
    {
        var userId = Context.User.GetUserId<long>();

        var userInCall = await callService.HasValue(userId);
        if (userInCall)
        {
            return;
        }

        var callExists = await callService.HasKey(call.ChatId);
        if (!callExists)
        {
            await callService.Add(call.ChatId, userId);
        }

        await callService.Add(call.ChatId, userId);

        await NotifyUsers(call.ChatId, call.ChatId, nameof(Calling));
    }

    public async Task JoinCall(JoinCall join)
    {
        var userId = Context.User.GetUserId<long>();

        var userInCall = await callService.HasValue(userId);
        if (userInCall)
        {
            return;
        }

        await callService.Add(join.ChatId, userId);

        await NotifyUsers(join.ChatId, join.PeerUserId, nameof(JoinCall));
    }

    public async Task LeaveCall(LeaveCall leave)
    {
        var userId = Context.User.GetUserId<long>();

        await callService.Delete(leave.ChatId, userId);

        var count = await callService.Count(leave.ChatId);

        if (count < 2)
        {
            await callService.Delete(leave.ChatId);
        }

        await NotifyUsers<string>(leave.ChatId, leave.PeerId, nameof(LeaveCall));
    }

    public async Task SendMessage(CreateMessageDto message)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.Create(message, userId);

        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0.ChatId, result.AsT0, nameof(SendMessage));
    }

    public async Task UpdateChat(UpdateChatDto userChat)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await userChatService.Update(userChat.ChatId, userId, userChat.LastRead);

        if (result.IsT1 || result.IsT2)
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
            var connections = await userConnections.Get(users);

            await Clients.Clients(connections)
                .SendAsync(method, message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.GetUserId<long>();

        await userConnections.Add(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User.GetUserId<long>();

        await userConnections.Delete(userId, Context.ConnectionId);

        if (await callService.HasValue(userId))
        {
            var chatId = await callService.Delete(userId);

            if (chatId.HasValue)
            {
                var count = await callService.Count(chatId.Value);

                if (count < 2)
                {
                    await callService.Delete(chatId.Value);
                }
            }

            _ = NotifyUsers(chatId.Value, chatId.Value, "LeaveCall");
        }

        await base.OnDisconnectedAsync(exception);
    }
}