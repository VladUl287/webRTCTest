namespace Web.Hubs.Core.Entities;

public sealed class Message
{
    public Guid Id { get; init; }

    public required string Content { get; set; }

    public required long UserId { get; init; }

    public required Guid ChatId { get; init; }

    public DateTime Date { get; init; } = DateTime.UtcNow;

    public bool Edit { get; set; }

    public Chat? Chat { get; init; }
}
