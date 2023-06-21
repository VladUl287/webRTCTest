using Web.Hubs.Core.Enums;

namespace Web.Hubs.Core.Dtos.Messages;

public sealed class CreateMessageDto
{
    public required Guid ChatId { get; init; }

    public required string Content { get; init; }

    public required MessageType Type { get; init; }
}