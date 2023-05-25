namespace Web.Hubs.Core.Dtos.Messages;

public sealed class MessageInfo
{
    public Guid Id { get; init; }

    public required string Content { get; init; }

    public required long UserId { get; init; }
    
    public required string Image { get; init; }

    public required Guid ChatId { get; init; }

    public required DateTime Date { get; init; }

    public required bool Edit { get; init; }
}