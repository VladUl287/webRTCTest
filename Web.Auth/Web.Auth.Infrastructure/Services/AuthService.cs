using OneOf;
using OneOf.Types;
using System.Text;
using Web.Auth.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using Web.Auth.Infrastructure.Database;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Web.Auth.Core.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Web.Auth.Core.Entities;
using Web.Auth.Core.Contracts;

namespace Web.Auth.Infrastructure.Service;

public sealed class AuthService : IAuthService
{
    private readonly DatabaseContext dbContext;
    private readonly ITokenService tokenService;
    private readonly IRedisService redisService;
    private readonly TokenConfiguration tokenConfiguration;
    private readonly PasswordConfiguration passwordConfiguration;

    public AuthService(
        DatabaseContext dbContext,
        ITokenService tokenService,
        IRedisService redisService,
        IOptions<TokenConfiguration> tokenOptions,
        IOptions<PasswordConfiguration> passwordOptions)
    {
        this.dbContext = dbContext;
        this.tokenService = tokenService;
        this.redisService = redisService;
        this.tokenConfiguration = tokenOptions.Value;
        this.passwordConfiguration = passwordOptions.Value;
    }

    public async Task<OneOf<ResultDto, NotFound>> Login(LoginDto loginDto)
    {
        var hash = GenerateHash(loginDto.Password, passwordConfiguration.SecretKey);

        var user = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Email == loginDto.Email && x.Password == hash)
            .Select(x => new
            {
                Id = x.Id,
                Email = x.Email
            })
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return new NotFound();
        }

        var identifier = user.Id.ToString();

        var result = GetCredentials(identifier, user.Email);

        await redisService.AddOrCreateValue<string>(identifier, result.RefreshToken);

        return result;
    }

    public async Task<OneOf<Success, Error<string>>> Registration(RegistrationDto registrationDto)
    {
        var exists = await dbContext.Users
            .AnyAsync(x => x.Email == registrationDto.Email);

        if (exists)
        {
            return new Error<string>("Пользователь с таким Email уже существует.");
        }

        var hash = GenerateHash(registrationDto.Password, passwordConfiguration.SecretKey);
        var user = new User
        {
            Email = registrationDto.Email,
            Name = registrationDto.Name,
            Password = hash
        };
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<OneOf<ResultDto, NotFound, Error>> Refresh(string token)
    {
        if (!tokenService.TryGetClaimValue(token, ClaimTypes.NameIdentifier, out string userId))
        {
            return new Error();
        }

        if (Guid.TryParse(userId, out Guid identifier))
        {
            return new Error();
        }

        var value = await redisService.GetAndDeleteValue<string>(userId, token);
        if (string.IsNullOrEmpty(value))
        {
            return new NotFound();
        }

        var email = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == identifier)
            .Select(x => x.Email)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(email))
        {
            await redisService.DeleteItem(userId);
            return new NotFound();
        }

        var result = GetCredentials(userId, email);

        await redisService.AddOrCreateValue<string>(userId, result.RefreshToken);

        return result;
    }

    public async Task Logout(Guid userId, string token)
    {
        await redisService.DeleteValue<string>(userId.ToString(), token);
    }

    public static string GenerateHash(string password, string key)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
             password: password,
             salt: Encoding.UTF8.GetBytes(key),
             prf: KeyDerivationPrf.HMACSHA256,
             iterationCount: 100000,
             numBytesRequested: 256 / 8));
    }

    private ResultDto GetCredentials(string userId, string email)
    {
        var accessExpires = DateTime.UtcNow.AddMinutes(tokenConfiguration.AccessExpires);
        var refreshExpires = DateTime.UtcNow.AddDays(tokenConfiguration.RefreshExpires);

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email)
        };

        var issuer = tokenConfiguration.Issuer;
        var audience = tokenConfiguration.Audience;
        var access = tokenConfiguration.AccessKey;
        var refresh = tokenConfiguration.RefreshKey;

        var accessToken = tokenService.Generate(claims, access, issuer, audience, accessExpires);
        var refreshToken = tokenService.Generate(claims, refresh, issuer, audience, refreshExpires);

        return new ResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}