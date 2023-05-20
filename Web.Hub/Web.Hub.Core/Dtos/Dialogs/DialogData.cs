namespace Web.Hub.Core.Dtos.Dialogs;

public sealed class DialogData
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Image { get; init; }

    public string LastMessage { get; init; } = string.Empty;
}
