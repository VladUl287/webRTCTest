namespace Web.Hubs.Core.Entities;

public sealed class ChatUser
{
    public required long UserId { get; init; }

    public string Name { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public DateTime LastRead { get; set; } = DateTime.UtcNow;

    public required Guid ChatId { get; init; }
    
    public Chat? Chat { get; init; }
}
