namespace Web.Hubs.Core.Entities;

public sealed class Message
{
    public Guid Id { get; init; }

    public required string Content { get; init; }

    public required long UserId { get; init; }

    public required Guid ChatId { get; init; }

    public required DateTime Date { get; init; }

    public bool Edit { get; init; }

    public Chat? Chat { get; init; }
}
