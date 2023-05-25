namespace Web.Hubs.Core.Dtos.Chats;

public sealed class ChatData
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Image { get; init; }

    public required DateTime LastRead { get; init; }

    public required string LastMessage { get; init; } = string.Empty;
}