using Web.Hubs.Core.Dtos;
using Web.Hubs.Core.Services;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs.Api.Hubs;

// [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore)]
public sealed class CallHub : Hub
{
    private readonly IStoreService<Guid, long> callStore;
    private readonly IStoreService<long, Guid> userCallStore;
    private readonly IStoreService<long, string> connectionsStore;

    private readonly IChatPresenter chatRepository;

    public CallHub(IStoreService<Guid, long> callStore, IStoreService<long, Guid> userCallStore, IStoreService<long, string> connectionsStore, IChatPresenter chatRepository)
    {
        this.callStore = callStore;
        this.userCallStore = userCallStore;
        this.connectionsStore = connectionsStore;
        this.chatRepository = chatRepository;
    }

    public async Task StartCall(StartCall room)
    {
        var userId = Context.User.GetUserId<long>();

        var users = await chatRepository.GetUsers(room.ChatId);
        if (users is null or { Length: < 2 })
        {
            return;
            // return false;
        }

        var inCall = await userCallStore.Has(userId);
        if (inCall)
        {
            return;
        }

        var callId = Guid.NewGuid();

        await userCallStore.Add(userId, callId);

        await callStore.Add(callId, userId);

        var connections = await connectionsStore.Get(users);

        await Clients.Clients(connections)
            .SendAsync("StartingCall", room.ChatId);
    }

    public async Task JoinCall(JoinCall join)
    {
        var userId = Context.User.GetUserId<long>();

        var inCall = await userCallStore.Has(userId);
        if (inCall)
        {
            //leave active call
            return;
        }

        await userCallStore.Add(userId, join.CallId);
        await callStore.Add(join.CallId, userId);

        await NotifyCallUsers(join.CallId, join.PeerUserId, "UserConnected");
    }

    public async Task LeaveCall(LeaveCall leave)
    {
        var userId = Context.User.GetUserId<long>();

        var inCall = await callStore.Has(leave.CallId, userId);

        if (inCall)
        {
            await callStore.Delete(leave.CallId, userId);
            await userCallStore.Delete(userId, leave.CallId);

            await NotifyCallUsers(leave.CallId, Guid.Empty, "UserDisconnected");
        }
    }

    private async Task NotifyCallUsers(Guid callId, Guid peerId, string method)
    {
        var users = await callStore.Get(callId);

        var connections = await connectionsStore.Get(users);

        await Clients.Clients(connections)
            .SendAsync(method, peerId);
    }
}