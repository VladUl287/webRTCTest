namespace Web.Hubs.Core.Dtos;

public sealed class CallDto
{
    public required Guid Id { get; init; }

    public long[] Users { get; init; } = Array.Empty<long>();
}