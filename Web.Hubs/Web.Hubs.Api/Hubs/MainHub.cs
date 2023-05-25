using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Messages;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Services;

namespace Web.Hubs.Api.Hubs;

// [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore)]
public sealed class MainHub : Hub
{
    private readonly IStoreService<long, string> usersStore;

    public MainHub()
    {
    }

    //OneOf<ChatData, NotFound>
    public async Task<ChatData?> GetChat(Guid chatId, [FromServices] IChatPresenter chatRepository)
    {
        var userId = Context.GetUserId<long>();

        return await chatRepository.GetChat(chatId, userId);
    }

    public async Task<IEnumerable<ChatData>> GetChats([FromServices] IChatPresenter chatRepository)
    {
        var userId = Context.GetUserId<long>();

        return await chatRepository.GetChats(userId);
    }

    public async Task<IEnumerable<MessageInfo>> GetMessages(Guid chatId, [FromServices] IMessagePresenter messagesRepository)
    {
        var userId = Context.GetUserId<long>();

        return await messagesRepository.GetMessages(chatId, userId);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetUserId<long>();

        await usersStore.Add(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetUserId<long>();

        await usersStore.Delete(userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}