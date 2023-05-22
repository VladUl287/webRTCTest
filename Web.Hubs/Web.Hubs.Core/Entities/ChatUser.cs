namespace Web.Hubs.Core.Entities;

public sealed class ChatUser
{
    public required Guid UserId { get; init; }

    public required Guid DialogId { get; init; }

    public required DateTime LastRead { get; init; }
}
