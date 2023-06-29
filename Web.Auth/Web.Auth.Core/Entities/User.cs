using Microsoft.AspNetCore.Identity;

namespace Web.Auth.Core.Entities;

public sealed class User : IdentityUser<long>
{
    public string Image { get; init; } = string.Empty;
}