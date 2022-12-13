using OneOf;
using OneOf.Types;
using Web.Auth.Core.Dtos;
using Web.Auth.Core.Contracts.Services;
using Web.Auth.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace Web.Auth.Infrastructure.Service;

public sealed class AuthService : IAuthService
{
    private readonly DatabaseContext dbContext;
    private readonly ITokenService tokenService;

    public AuthService(DatabaseContext dbContext, ITokenService tokenService)
    {
        this.dbContext = dbContext;
        this.tokenService = tokenService;
    }

    public async Task<OneOf<ResultDto, Error<string>>> Login(LoginDto loginDto)
    {
        var hash = "";

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == loginDto.Email && x.Password == hash);

        if (user is null)
        {
            return new Error<string>("");
        }

        // var accessToken = tokenService.Generate(user.Id, user.Email);
        // var refreshToken = tokenService.Generate(user.Id, user.Email);

        return new ResultDto
        {
            AccessToken = "",
            RefreshToken = ""
        };
    }

    public Task<OneOf<ResultDto, Error>> Registration(RegistrationDto registrationDto)
    {
        throw new NotImplementedException();
    }

    public static string Hash(string password, string key)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
             password: password,
             salt: Encoding.UTF8.GetBytes(key),
             prf: KeyDerivationPrf.HMACSHA256,
             iterationCount: 100000,
             numBytesRequested: 256 / 8));
    }
}