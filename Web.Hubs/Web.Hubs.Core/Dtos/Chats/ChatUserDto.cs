namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatUserDto
{
    public required long UserId { get; init; }

    public required DateTime LastRead { get; init; }
}