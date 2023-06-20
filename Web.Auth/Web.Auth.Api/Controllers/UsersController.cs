using Web.Auth.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Core.Repositories;
using Web.Auth.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace Web.Auth.Api.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("/api/[controller]/[action]")]
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
