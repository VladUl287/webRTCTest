namespace Web.Hubs.Core.Dtos.Messages;

public sealed class MessageInfo
{
    public Guid Id { get; init; }

    public required string Content { get; init; }

    public required long UserId { get; init; }

    public required Guid DialogId { get; init; }

    public required DateTime Date { get; init; }
}