namespace Web.Hubs.Core.Dtos.UserChat;

public sealed class UpdateChatDto
{
    public Guid ChatId { get; init; }

    public DateTime LastRead { get; init; }
}
