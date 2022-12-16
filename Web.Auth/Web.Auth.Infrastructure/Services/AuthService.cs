using OneOf;
using OneOf.Types;
using System.Text;
using Web.Auth.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using Web.Auth.Core.Contracts.Services;
using Web.Auth.Infrastructure.Database;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Web.Auth.Core.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Web.Auth.Core.Entities;

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
        var accessExpires = DateTime.Now.AddMinutes(tokenConfiguration.AccessExpires);
        var refreshExpires = DateTime.Now.AddDays(tokenConfiguration.RefreshExpires);
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, identifier),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var issuer = tokenConfiguration.Issuer;
        var audience = tokenConfiguration.Audience;
        var accessToken = tokenService.Generate(claims, tokenConfiguration.AccessKey, issuer, audience, accessExpires);
        var refreshToken = tokenService.Generate(claims, tokenConfiguration.RefreshKey, issuer, audience, refreshExpires);

        await redisService.SetItem<string>(identifier, refreshToken);

        return new ResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
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

    public async Task<OneOf<ResultDto, NotFound, Error>> Refresh(Guid userId, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return new Error();
        }

        var identifier = userId.ToString();
        var item = await redisService.GetItem<string>(identifier, token);
        if (string.IsNullOrEmpty(item))
        {
            return new NotFound();
        }

        var email = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.Email)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(email))
        {
            await redisService.DeleteItem(identifier);
            return new NotFound();
        }

        var accessExpires = DateTime.Now.AddMinutes(tokenConfiguration.AccessExpires);
        var refreshExpires = DateTime.Now.AddDays(tokenConfiguration.RefreshExpires);
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, identifier),
            new Claim(ClaimTypes.Email, email)
        };
        var issuer = tokenConfiguration.Issuer;
        var audience = tokenConfiguration.Audience;
        var accessToken = tokenService.Generate(claims, tokenConfiguration.AccessKey, issuer, audience, accessExpires);
        var refreshToken = tokenService.Generate(claims, tokenConfiguration.RefreshKey, issuer, audience, refreshExpires);

        await redisService.SetItem<string>(identifier, refreshToken);

        return new ResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task Logout(Guid userId, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return;
        }

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

}