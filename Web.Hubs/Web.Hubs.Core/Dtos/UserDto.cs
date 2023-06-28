namespace Web.Hubs.Core.Dtos;

public sealed class UserDto
{
    public long Id { get; init; }

    public string Image { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;
}
