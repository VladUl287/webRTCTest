using OneOf;
using OneOf.Types;
using Web.Auth.Core.Dtos;

namespace Web.Auth.Core.Contracts.Services;

public interface IAuthService
{
    Task<OneOf<ResultDto, Error<string>>> Login(LoginDto loginDto);

    Task<OneOf<ResultDto, Error>> Registration(RegistrationDto registrationDto);
}