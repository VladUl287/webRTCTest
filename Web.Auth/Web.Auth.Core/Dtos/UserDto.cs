namespace Web.Auth.Core.Dtos;

public sealed class UserDto
{
    public long Id { get; init; }

    public Guid ChatId { get; set; }

    public string UserName { get; init; } = string.Empty;
}
