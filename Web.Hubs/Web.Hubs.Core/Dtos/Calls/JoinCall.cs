namespace Web.Hubs.Core.Dtos;

public sealed class JoinCall
{
    public required Guid ChatId { get; init; }

    public required Guid PeerId { get; init; }
}