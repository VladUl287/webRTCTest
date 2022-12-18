using System.Net.Cache;
namespace Web.Auth.Core.Dtos;

public sealed class UserDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
}