namespace Web.Hubs.Core.Entities;

public sealed class ChatUser
{
    public required long UserId { get; init; }

    public required Guid ChatId { get; init; }

    public required DateTime LastRead { get; init; }

    public Chat? Chat { get; init; }
}
