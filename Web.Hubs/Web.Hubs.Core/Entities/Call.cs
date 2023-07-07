namespace Web.Hubs.Core.Entities;

public sealed class Call
{
    public required Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public IEnumerable<CallUser>? CallUsers { get; init; }
}
