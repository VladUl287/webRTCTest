namespace Web.Hub.Core.Dtos.Dialogs;

public sealed class DialogCreate
{
    public required string Name { get; init; }

    public required long UserId { get; init; }

    public required string Image { get; init; }

    public required DialogType Type { get; init; }

    public required AccessType Access { get; init; }

    public required IEnumerable<UserData> Users { get; init; } = Enumerable.Empty<UserData>();
}