using System.Security.Claims;
using Web.Auth.Core.Configuration;

namespace Web.Auth.Core.Contracts.Services;

public interface ITokenService
{
    string Generate(Claim[] claims, string key, string issuer, string audience, DateTime expires);

    bool ValidateToken(string token, string key, string issuer, string audience);
}