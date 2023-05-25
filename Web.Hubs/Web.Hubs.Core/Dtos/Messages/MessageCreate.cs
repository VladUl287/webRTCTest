namespace Web.Hubs.Core.Dtos.Messages;

public sealed class MessageCreate
{
    public required string Content { get; init; }

    public required Guid ChatId { get; init; }
}