using Web.Auth.Core.Dtos;
using Web.Auth.Api.Extentions;
using Web.Auth.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Auth.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    private const string REFRESH_TOKEN = "refresh_token";

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
    {
        var result = await authService.Login(loginDto);
        if (result.IsT1)
        {
            return NotFound();
        }

        var loginResult = result.AsT0;
        Response.Cookies.Append(REFRESH_TOKEN, loginResult.RefreshToken, new CookieOptions
        {
            Expires = new DateTimeOffset(DateTime.UtcNow.AddDays(30))
        });

        return Ok(loginResult);
    }

    [HttpPost]
    public async Task<IActionResult> Registration([FromForm] RegistrationDto registrationDto)
    {
        var result = await authService.Registration(registrationDto);

        return result.Match<IActionResult>(
            success => Ok(),
            error => BadRequest(error)
        );
    }

    [HttpPut]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[REFRESH_TOKEN];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        var result = await authService.Refresh(refreshToken);

        return result.Match<IActionResult>(
            success => Ok(success),
            notFound => NotFound(),
            error => BadRequest(error)
        );
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[REFRESH_TOKEN];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        var userId = User.GetLoggedInUserId<Guid>();
        await authService.Logout(userId, refreshToken);

        Response.Cookies.Delete(REFRESH_TOKEN);

        return NoContent();
    }
}