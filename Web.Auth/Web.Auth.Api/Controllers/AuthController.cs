using Web.Auth.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Core.Contracts.Services;

namespace Web.Auth.Api.Controllers;

public sealed class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
    {
        var result = await authService.Login(loginDto);

        return result.Match<IActionResult>(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Registration([FromForm] RegistrationDto registrationDto)
    {
        var result = await authService.Registration(registrationDto);

        return result.Match<IActionResult>(
            success => Ok(success),
            error => BadRequest(error)
        );
    }
}