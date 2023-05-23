namespace Web.Hubs.Core.Dtos.Messages;

public sealed class MessageUpdate
{
    public Guid Id { get; init; }

    public required string Content { get; init; }
}