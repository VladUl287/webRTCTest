namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatUpdate
{
    public required Guid Id { get; init; }

    public string? Name { get; init; }

    public string? Image { get; init; }
}