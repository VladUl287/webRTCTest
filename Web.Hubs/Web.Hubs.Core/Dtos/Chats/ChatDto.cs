using Web.Hubs.Core.Dtos.Messages;
using Web.Hubs.Core.Enums;

namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatDto
{
    public required Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public DateTime Date { get; init; }

    public long UserId { get; set; }

    public DateTime? LastRead { get; init; }

    public MessageDto? Message { get; init; }

    public int? Unread { get; init; }
}