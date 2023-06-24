using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public string Image { get; init; } = string.Empty;

    public MessageDto? Message { get; init; }

    public int? Unread { get; init; }
}