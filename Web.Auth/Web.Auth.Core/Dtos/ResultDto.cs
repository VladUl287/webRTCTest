namespace Web.Auth.Core.Dtos;

public sealed class ResultDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}