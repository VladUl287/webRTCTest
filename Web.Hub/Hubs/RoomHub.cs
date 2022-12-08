using Microsoft.AspNetCore.SignalR;

namespace WebHub.App.Hubs;

public sealed class RoomHub : Hub
{
    public async Task Join(RoomOptions room)
    {
        await Groups.AddToGroupAsync(room.UserId.ToString(), room.RoomId.ToString());
        await Clients.Group(room.RoomId.ToString()).SendAsync("connected", room.UserId);
    }
}