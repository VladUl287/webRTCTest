namespace Web.Hub.Core;

public sealed class Message
{
    public Guid Id { get; init; }

    public required string Content { get; init; }

    public required long UserId { get; init; }

    public required Guid DialogId { get; init; }

    public required DateTime Date { get; init; }

    public Dialog? Dialog { get; init; }
}
