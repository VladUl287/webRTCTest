namespace Web.Hubs.Core.Dtos.Chats;

public sealed class UpdateChatDto
{
    public required Guid Id { get; init; }

    public string? Name { get; init; }

    public string? Image { get; init; }
}