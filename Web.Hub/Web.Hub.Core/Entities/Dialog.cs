namespace Web.Hub.Core;

public sealed class Dialog
{
    public Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Image { get; set; }

    public required DateTime Date { get; init; }

    public required DialogType Type { get; init; }

    public required AccessType Access { get; init; }

    public required long UserId { get; init; }
}
