namespace Web.Hubs.Core.Dtos.Messages;

public sealed class LastMessageDto
{
    public required string Content { get; init; }

    public required DateTime Date { get; init; }
}