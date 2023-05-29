namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatData
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Image { get; init; }

    public int Unread { get; init; }

    public string? LastMessage { get; init; } = string.Empty;
}