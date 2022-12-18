using OneOf;
using OneOf.Types;
using Web.Auth.Core.Dtos;

namespace Web.Auth.Core.Contracts;

public interface IAuthService
{
    Task<OneOf<ResultDto, NotFound>> Login(LoginDto loginDto);

    Task<OneOf<Success, Error<string>>> Registration(RegistrationDto registrationDto);

    Task<OneOf<ResultDto, NotFound, Error>> Refresh(string token);

    Task Logout(Guid userId, string token);
}