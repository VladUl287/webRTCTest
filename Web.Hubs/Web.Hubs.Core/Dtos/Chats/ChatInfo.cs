using Web.Hubs.Core.Enums;

namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatInfo
{
    public required string Name { get; init; }

    public required string Image { get; init; }

    public required long UserId { get; init; }

    public required DateTime Date { get; init; }

    public required ChatType Type { get; init; }

    public required IEnumerable<UserData> Users { get; init; } = Enumerable.Empty<UserData>();
}
