namespace Web.Auth.Core.Dtos;

public sealed class UsersFilter
{
    public string? Name { get; init; }

    public PageFilter? PageFilter { get; init; }
}
