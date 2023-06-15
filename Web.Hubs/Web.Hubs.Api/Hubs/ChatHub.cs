using Web.Hubs.Core.Services;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Web.Hubs.Core.Dtos.UserChat;

namespace Web.Hubs.Api.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IUserChatService userChatService;
    private readonly IChatPresenter chatPresenter;
    private readonly IMessageService messageService;
    private readonly IConnectionService connectionStore;

    public ChatHub(IChatPresenter chatPresenter, IMessageService messageService, IConnectionService connectionStore, IUserChatService userChatService)
    {
        this.chatPresenter = chatPresenter;
        this.messageService = messageService;
        this.connectionStore = connectionStore;
        this.userChatService = userChatService;
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