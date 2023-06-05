using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Image { get; init; }

    public MessageDto? Message { get; init; }

    public int? Unread { get; init; }
}