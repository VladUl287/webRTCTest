using Web.Hubs.Core.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs.Api.Hubs;

public sealed class RoomHub : Hub
{
    public async Task Join(RoomOptions room)
    {
        await Clients.All.SendAsync("connected", room.UserPeerId);
    }
}