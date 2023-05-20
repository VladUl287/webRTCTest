namespace Web.Hub.Core.Dtos.Messages;

public sealed class MessageCreate
{
    public required string Content { get; init; }

    public required long UserId { get; init; }

    public required Guid DialogId { get; init; }
}