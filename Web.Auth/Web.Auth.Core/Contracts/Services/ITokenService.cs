using System.Security.Claims;
using Web.Auth.Core.Configuration;

namespace Web.Auth.Core.Contracts.Services;

public interface ITokenService
{
    string Generate(Claim[] claims, DateTime expires, in TokenConfiguration configuration);

    bool ValidateToken(string token, in TokenConfiguration configuration);
}