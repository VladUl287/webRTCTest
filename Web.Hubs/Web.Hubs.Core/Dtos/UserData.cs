namespace Web.Hubs.Core.Dtos;

public sealed class UserData
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;
}