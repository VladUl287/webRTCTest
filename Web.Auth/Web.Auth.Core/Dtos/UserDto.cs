namespace Web.Auth.Core.Dtos;

public sealed class UserDto
{
    public long Id { get; init; }

    public Guid? ChatId { get; set; } = null;

    public string UserName { get; init; } = string.Empty;

    public string Image { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;
}
