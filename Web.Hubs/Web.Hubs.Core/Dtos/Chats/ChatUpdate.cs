namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatUpdate
{
    public Guid Id { get; init; }

    public required long UserId { get; init; }

    public required string Name { get; init; }

    public required string Image { get; init; }
}