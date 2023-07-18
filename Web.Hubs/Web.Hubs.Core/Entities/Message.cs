namespace Web.Hubs.Core.Entities;

public sealed class Message
{
    public Guid Id { get; init; }

    public required string Content { get; init; }

    public required long UserId { get; init; }

    public DateTime Date { get; init; } = DateTime.UtcNow;

    public bool Edit { get; set; }

    public required Guid ChatId { get; init; }

    public Chat? Chat { get; init; }
}
