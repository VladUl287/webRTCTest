namespace Web.Auth.Core.Configuration;

public sealed class PasswordConfiguration
{
    public const string Position = "PasswordConfiguration";

    public required string SecretKey { get; init; }
}