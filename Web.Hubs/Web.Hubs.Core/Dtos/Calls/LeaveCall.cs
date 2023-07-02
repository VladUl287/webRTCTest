namespace Web.Hubs.Core.Dtos;

public sealed class LeaveCall
{
    public required Guid ChatId { get; init; }

    public required long UserId { get; init; }

    public required string PeerId { get; init; }
}