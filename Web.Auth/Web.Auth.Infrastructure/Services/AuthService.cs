using OneOf;
using OneOf.Types;
using Web.Auth.Core.Contracts.Services;
using Web.Auth.Core.Dtos;

namespace Web.Auth.Infrastructure.Service;

public sealed class AuthService : IAuthService
{
    public Task<OneOf<ResultDto, Error>> Login(LoginDto loginDto)
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<ResultDto, Error>> Registration(RegistrationDto registrationDto)
    {
        throw new NotImplementedException();
    }
}