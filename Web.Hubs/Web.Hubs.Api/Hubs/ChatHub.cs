using Web.Hubs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs.Api.Hubs;

// [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore)]
public sealed class ChatHub : Hub
{
    private readonly IStoreService<long> store;
    private readonly IMessageService messageService;

    public ChatHub(IStoreService<long> store, IMessageService messageService)
    {
        this.store = store;
        this.messageService = messageService;
    }

    // OneOf<MessageInfo, Error<string>>
    public async Task<bool> SendMessage(MessageCreate message, [FromServices] IChatPresenterRepository chatRepository)
    {
        if (!long.TryParse(Context?.User?.Identity?.Name, out long userId) || userId != message.UserId)
        {
            return false;
        }

        var result = await messageService.CreateMessage(message);
        if (result.IsT0)
        {
            var chatUsersIds = await chatRepository.GetUsersIds(message.ChatId);

            if (chatUsersIds is not null and { Length: > 0 })
            {
                var connections = await store.Get(chatUsersIds);

                await Clients.Clients(connections)
                    .SendAsync("ReceiveMessage", message);
            }

            return true;
        }

        return false;
    }

    //OneOf<MessageInfo, NotFound, Error<string>>
    public async Task<bool> UpdateMessage(MessageUpdate update)
    {
        if (!long.TryParse(Context?.User?.Identity?.Name, out long userId))
        {
            return false;
        }

        var result = await messageService.UpdateMessage(update.Id, userId, update.Content);

        return result.Match(
            success => true,
            notFound => false
        );
    }

    //OneOf<Success, NotFound>
    public async Task<bool> DeleteMessage(Guid messageId)
    {
        if (!long.TryParse(Context?.User?.Identity?.Name, out long userId))
        {
            return false;
        }

        var result = await messageService.DeleteMessage(messageId, userId);

        return result.Match(
            success => true,
            notFound => false
        );
    }

    public override async Task OnConnectedAsync()
    {
        if (long.TryParse(Context?.User?.Identity?.Name, out long userId))
        {
            await store.Add(userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (long.TryParse(Context?.User?.Identity?.Name, out long userId))
        {
            await store.Delete(userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}