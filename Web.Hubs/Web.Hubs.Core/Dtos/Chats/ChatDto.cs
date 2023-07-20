using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatDto
{
    public required Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public long UserId { get; set; }

    public DateTime Date { get; init; }

    public DateTime? LastRead { get; init; }

    public LastMessageDto? LastMessage { get; init; }

    public int? Unread { get; init; }

    // public ChatUserDto[] ChatUsers { get; init; } = Array.Empty<ChatUserDto>();
}