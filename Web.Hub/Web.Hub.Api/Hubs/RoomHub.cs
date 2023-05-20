using Web.Hub.Core.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace WebHub.App.Hubs;

public sealed class RoomHub : Hub
{
    public async Task Join(RoomOptions room)
    {
        await Clients.All.SendAsync("connected", room.UserPeerId);
    }
}