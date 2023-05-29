using Web.Hubs.Core.Enums;

namespace Web.Hubs.Core.Entities;

public sealed class Chat
{
    public Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Image { get; set; }

    public required DateTime Date { get; init; }

    public required ChatType Type { get; init; }

    public required long UserId { get; init; }

    public IEnumerable<Message> Messages { get; init; } = Enumerable.Empty<Message>();

    public IEnumerable<ChatUser> ChatUsers { get; set; } = Enumerable.Empty<ChatUser>();
}
