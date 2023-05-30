namespace Web.Hubs.Core.Entities;

public sealed class ChatUser
{
    public required long UserId { get; init; }

    public required Guid ChatId { get; init; }

    public DateTime LastRead { get; init; } = DateTime.UtcNow;

    public Chat? Chat { get; init; }
}
