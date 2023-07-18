namespace Web.Hubs.Api.Configuration;

public sealed class CorsConfiguration
{
    public const string Position = "CorsConfiguration";

    public required string[] Origins { get; init; }
}