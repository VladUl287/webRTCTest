namespace Web.Hubs.Core.Dtos;

public sealed class CallDto
{
    public required Guid ChatId { get; init; }

    public string Name { get; init; }

    public long[] Users { get; init; }
}