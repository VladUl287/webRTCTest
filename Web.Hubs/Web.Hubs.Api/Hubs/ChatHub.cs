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
    private readonly IUserChatService userChatService;
    private readonly IChatPresenter chatPresenter;
    private readonly IMessageService messageService;
    private readonly IConnectionService connectionStore;
    private readonly ICallService callService;

    public ChatHub(IChatPresenter chatPresenter, IMessageService messageService, IConnectionService connectionStore, IUserChatService userChatService, ICallService callService)
    {
        this.chatPresenter = chatPresenter;
        this.messageService = messageService;
        this.connectionStore = connectionStore;
        this.userChatService = userChatService;
        this.callService = callService;
    }

    public async Task StartCall(StartCall call)
    {
        var userId = Context.User.GetUserId<long>();

        var users = await chatPresenter.GetUsers(call.ChatId);
        if (users is null or { Length: < 2 })
        {
            return;
        }

        // var exists = await callService.UserExists(userId);
        // if (exists)
        // {
        //     return;
        // }

        // await callService.AddUser(call.ChatId, userId);

        var connections = await connectionStore.Get(users);

        await Clients.Clients(connections)
            .SendAsync("StartingCall", call.ChatId);
    }

    public async Task JoinCall(JoinCall join)
    {
        var userId = Context.User.GetUserId<long>();

        // var exists = await callService.UserExists(userId);
        // if (exists)
        // {
        //     return;
        // }

        // await callService.AddUser(join.ChatId, userId);

        await NotifyUsers(join.ChatId, join.PeerUserId, "JoinedCall");
    }

    public async Task LeaveCall(LeaveCall leave)
    {
        var userId = Context.User.GetUserId<long>();

        var exists = await callService.UserExists(leave.ChatId, userId);

        if (exists)
        {
            await callService.RemoveUser(leave.ChatId, userId);

            await NotifyUsers(leave.ChatId, Guid.Empty, "LeaveCall");
        }
    }

    private async Task NotifyUsers(Guid chatId, Guid peerId, string method)
    {
        var users = await chatPresenter.GetUsers(chatId);

        if (users?.Length > 0)
        {
            var connections = await connectionStore.Get(users);

            await Clients.Clients(connections)
                .SendAsync(method, peerId);
        }
    }

    public async Task SendMessage(CreateMessageDto message)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.Create(message, userId);

        if (result.IsT1)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("SendingError");

            return;
        }

        await NotifyUsers(result.AsT0, "ReceivedMessage");
    }

    public async Task UpdateMessage(UpdateMessageDto update)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.Update(update, userId);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0, "UpdatedMessage");
    }

    public async Task DeleteMessage(Guid messageId)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.Delete(messageId, userId);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0, "DeletedMessage");
    }

    public async Task ChatCreated(Guid chatId)
    {
        var users = await chatPresenter.GetUsers(chatId);

        if (users?.Length > 0)
        {
            var connections = await connectionStore.Get(users);

            await Clients.Clients(connections)
                .SendAsync("ChatCreated", chatId);
        }
    }

    public async Task UpdateUserChat(UpdateChatUserDto userChat)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await userChatService.Update(userChat.ChatId, userId, userChat.LastRead);

        if (result.IsT1 || result.IsT2)
        {
            return;
        }

        var users = await chatPresenter.GetUsers(userChat.ChatId);

        if (users?.Length > 0)
        {
            var connections = await connectionStore.Get(users);

            await Clients.Clients(connections)
                .SendAsync("UpdatedChat", userChat.ChatId);
        }
    }

    private async Task NotifyUsers(MessageDto message, string method)
    {
        var users = await chatPresenter.GetUsers(message.ChatId);

        if (users?.Length > 0)
        {
            var connections = await connectionStore.Get(users);

            await Clients.Clients(connections)
                .SendAsync(method, message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.GetUserId<long>();

        await connectionStore.Add(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User.GetUserId<long>();

        await connectionStore.Delete(userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}