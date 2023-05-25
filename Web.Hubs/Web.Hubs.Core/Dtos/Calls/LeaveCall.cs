namespace Web.Hubs.Core.Dtos;

public sealed class LeaveCall
{
    public required Guid CallId { get; init; }

    public required long UserId { get; init; }
}