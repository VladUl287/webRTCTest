namespace Web.Hubs.Core.Entities;

public sealed class CallUser
{
    public required Guid CallId { get; init; }

    public required long UserId { get; init; }

    public Call? Call { get; init; }
}
