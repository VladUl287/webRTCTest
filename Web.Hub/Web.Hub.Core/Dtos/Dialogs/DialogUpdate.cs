namespace Web.Hub.Core.Dtos.Dialogs;

public sealed class DialogUpdate
{
    public Guid Id { get; init; }

    public required long UserId { get; init; }

    public required string Name { get; init; }

    public required string Image { get; init; }

    public required AccessType Access { get; init; }
}