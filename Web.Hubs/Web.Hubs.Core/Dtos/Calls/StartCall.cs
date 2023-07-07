namespace Web.Hubs.Core.Dtos;

public sealed class StartCall
{
    public required Guid ChatId { get; init; }

    public required long UserId { get; init; }
}