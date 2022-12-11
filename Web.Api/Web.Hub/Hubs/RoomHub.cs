using Web.Hub.Core.Store;
using WebHub.App.Core.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace WebHub.App.Hubs;

public sealed class RoomHub : Hub
{
    private readonly static ConnectionStore<string> Rooms = new();

    public async Task Join(RoomOptions room)
    {
        Rooms.Add(room.RoomId, room.UserPeerId);

        await Clients.All.SendAsync("connected", room.UserPeerId);
    }
}