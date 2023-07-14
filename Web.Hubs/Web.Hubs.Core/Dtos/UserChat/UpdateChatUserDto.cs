namespace Web.Hubs.Core.Dtos.UserChat;

public sealed class UpdateChatUserDto
{
    public Guid ChatId { get; init; }

    public DateTime LastRead { get; init; }
}
