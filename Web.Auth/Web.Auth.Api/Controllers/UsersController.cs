using Web.Auth.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Core.Repositories;
using Web.Auth.Infrastructure.Proxies;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Auth.Api.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersRepository usersRepository;
    private readonly IHubApi hubApi;

    public UsersController(IUsersRepository usersRepository, IHubApi hubApi)
    {
        this.usersRepository = usersRepository;
        this.hubApi = hubApi;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserInfo([FromQuery][BindRequired] long userId)
    {
        var user = await usersRepository.GetUser(userId);

        if (user is null)
        {
            return NotFound(userId);
        }

        return Ok(user);
    }

    [HttpGet]
    public async Task<IEnumerable<UserDto>> GetUsers([FromQuery] UsersFilter? filters = null)
    {
        var users = await usersRepository.GetUsers(filters);

        for (int i = 0; i < users.Length; i++)
        {
            var response = await hubApi.GetChatId(users[i].Id, 1);

            if (response.IsSuccessStatusCode)
            {
                users[i].ChatId = response.Content;
            }
        }

        return users;
    }
}
