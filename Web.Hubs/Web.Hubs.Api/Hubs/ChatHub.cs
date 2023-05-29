using Web.Hubs.Core.Services;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs.Api.Extensions;

namespace Web.Hubs.Api.Hubs;

// [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore)]
public sealed class ChatHub : Hub
{
    private readonly IChatPresenter chatPresenter;
    private readonly IMessageService messageService;
    private readonly IStoreService<long, string> usersStore;

    public ChatHub(IChatPresenter chatPresenter, IMessageService messageService, IStoreService<long, string> usersStore)
    {
        this.messageService = messageService;
        this.chatPresenter = chatPresenter;
        this.usersStore = usersStore;
    }

    public async Task SendMessage(MessageCreate message)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.CreateMessage(message, userId);
        if (result.IsT1)
        {
            return;
            // return false;
        }

        await NotifyUsers(result.AsT0, "ReceiveMessage");
    }

    public async Task UpdateMessage(MessageUpdate update)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.UpdateMessage(update.Id, userId, update.Content);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0, "UpdateMessage");
    }

    public async Task DeleteMessage(Guid messageId)
    {
        var userId = Context.User.GetUserId<long>();

        var result = await messageService.DeleteMessage(messageId, userId);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0, "DeleteMessage");
    }

    private async Task NotifyUsers(MessageData message, string method)
    {
        var users = await chatPresenter.GetUsers(message.ChatId);

        if (users?.Length > 0)
        {
            var connections = await usersStore.Get(users);

            await Clients.Clients(connections)
                .SendAsync(method, message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.GetUserId<long>();

        await usersStore.Add(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User.GetUserId<long>();

        await usersStore.Delete(userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}