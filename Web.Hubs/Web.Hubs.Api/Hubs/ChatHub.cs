using Web.Hubs.Core.Services;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs.Api.Extensions;

namespace Web.Hubs.Api.Hubs;

// [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore)]
public sealed class ChatHub : Hub
{
    private readonly IStoreService<long, string> usersStore;
    private readonly IMessageService messageService;
    private readonly IChatPresenter chatPresenter;

    public ChatHub()
    {
    }

    //OneOf<MessageInfo, Error>
    public async Task SendMessage(MessageCreate message)
    {
        var userId = Context.GetUserId<long>();

        var result = await messageService.CreateMessage(message, userId);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0, "ReceiveMessage");
    }

    //OneOf<MessageInfo, Error>
    public async Task UpdateMessage(MessageUpdate update)
    {
        var userId = Context.GetUserId<long>();

        var result = await messageService.UpdateMessage(update.Id, userId, update.Content);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0, "UpdateMessage");
    }

    //OneOf<Success, NotFound>
    public async Task DeleteMessage(Guid messageId)
    {
        var userId = Context.GetUserId<long>();

        var result = await messageService.DeleteMessage(messageId, userId);
        if (result.IsT1)
        {
            return;
        }

        await NotifyUsers(result.AsT0, "DeleteMessage");
    }

    private async Task NotifyUsers(MessageInfo message, string method)
    {
        var users = await chatPresenter.GetUsers(message.ChatId);

        if (users?.Length > 0)
        {
            var connections = await usersStore.Get(users);

            await Clients.Clients(connections)
                .SendAsync(method, message);
        }
    }
}