namespace Web.Hubs.Core.Entities;

public sealed class Call
{
    public required Guid Id { get; init; }

    public IEnumerable<CallUser> CallUsers { get; init; } = Enumerable.Empty<CallUser>();
}
