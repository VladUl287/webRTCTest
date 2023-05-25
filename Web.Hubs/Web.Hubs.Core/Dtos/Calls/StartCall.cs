namespace Web.Hubs.Core.Dtos;

public sealed class StartCall
{
    public required Guid PeerUserId { get; init; }

    public required Guid ChatId { get; init; }
}