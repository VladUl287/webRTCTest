namespace Web.Hubs.Core.Dtos.Messages;

public sealed class UpdateMessageDto
{
    public required Guid Id { get; init; }

    public required string Content { get; init; }
}