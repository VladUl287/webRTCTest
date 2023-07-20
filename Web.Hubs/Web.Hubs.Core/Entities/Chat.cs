using Web.Hubs.Core.Enums;

namespace Web.Hubs.Core.Entities;

public sealed class Chat
{
    public Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Image { get; set; }

    public required long UserId { get; init; }

    public required ChatType Type { get; init; }

    public DateTime Date { get; init; } = DateTime.UtcNow;

    public ICollection<Message>? Messages { get; init; }

    public ICollection<ChatUser>? ChatUsers { get; init; }
}
