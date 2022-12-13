using Web.Auth.Core.Configuration;

namespace Web.Auth.Core.Contracts.Services;

public interface ITokenService
{
    string Generate(Guid id, string email, TokenConfiguration configuration);

    bool ValidateToken(string token, TokenConfiguration configuration);
}