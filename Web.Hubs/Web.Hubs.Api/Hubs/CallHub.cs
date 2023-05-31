using Web.Hubs.Core.Dtos;
using Web.Hubs.Core.Services;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs.Api.Hubs;

// [Authorize]
public sealed class CallHub : Hub
{
    private readonly ICallService callService;
    private readonly IChatPresenter chatPresenter;
    private readonly IConnectionService connectionService;

    public CallHub()
    { }

    public async Task StartCall(StartCall call)
    {
        var userId = Context.User.GetUserId<long>();

        var users = await chatPresenter.GetUsers(call.ChatId);
        if (users is null or { Length: < 2 })
        {
            return;
        }

        var exists = await callService.UserExists(userId);
        if (exists)
        {
            return;
        }

        await callService.AddUser(call.ChatId, userId);

        var connections = await connectionService.Get(users);

        await Clients.Clients(connections)
            .SendAsync("StartingCall", call.ChatId);
    }

    public async Task JoinCall(JoinCall join)
    {
        var userId = Context.User.GetUserId<long>();

        var exists = await callService.UserExists(userId);
        if (exists)
        {
            return;
        }

        await callService.AddUser(join.ChatId, userId);

        await NotifyUsers(join.ChatId, join.PeerUserId, "UserConnected");
    }

    public async Task LeaveCall(LeaveCall leave)
    {
        var userId = Context.User.GetUserId<long>();

        var exists = await callService.UserExists(leave.ChatId, userId);

        if (exists)
        {
            await callService.RemoveUser(leave.ChatId, userId);

            await NotifyUsers(leave.ChatId, Guid.Empty, "UserDisconnected");
        }
    }

    private async Task NotifyUsers(Guid chatId, Guid peerId, string method)
    {
        var users = await chatPresenter.GetUsers(chatId);

        if (users?.Length > 0)
        {
            var connections = await connectionService.Get(users);

            await Clients.Clients(connections)
                .SendAsync(method, peerId);
        }
    }
}