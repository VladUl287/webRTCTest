namespace Web.Files.Options;

public abstract class FileBaseOptions
{
    public required uint MaxBytes { get; init; }

    public required uint MinBytes { get; init; }

    public required string[] Extensions { get; init; }
}
