namespace Web.Hub.Core;

public sealed class DialogUser
{
    public required Guid DialogId { get; init; }

    public required Guid UserId { get; init; }

    public required DateTime LastRead { get; init; }

    public required UserRights Right { get; init; }
}
