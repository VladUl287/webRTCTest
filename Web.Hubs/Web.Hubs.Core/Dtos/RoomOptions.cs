namespace Web.Hubs.Core.Dtos;

public sealed class RoomOptions
{
    public required string RoomId { get; init; }

    public required string UserPeerId { get; init; }

    public required Guid ChatId { get; init; }
}