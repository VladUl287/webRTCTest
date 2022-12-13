namespace Web.Auth.Core.Configuration;

public sealed class TokenConfiguration
{
    public required string SecurityKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required DateTime Expires { get; init; }
}