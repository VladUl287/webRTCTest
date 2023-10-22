namespace Web.Hubs.Core.Entities;

public sealed class Call
{
    public Guid Id { get; init; }

    public ICollection<CallUser>? CallUsers { get; init; }
}
