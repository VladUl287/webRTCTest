namespace Web.Auth.Core.Configuration;

public sealed class TokenConfiguration
{
    public const string Position = "TokenConfiguration";

    public required string AccessKey { get; init; }
    public required string RefreshKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int AccessExpires { get; init; }
    public required int RefreshExpires { get; init; }
}