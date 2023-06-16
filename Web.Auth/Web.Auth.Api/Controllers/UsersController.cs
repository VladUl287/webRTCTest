using Web.Auth.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Core.Repositories;

namespace Web.Auth.Api;

[ApiController]
[Route("/api/[controller]/[action]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersRepository usersRepository;

    public UsersController(IUsersRepository usersRepository)
    {
        this.usersRepository = usersRepository;
    }

    [HttpGet]
    public Task<UserDto[]> GetUsers([FromQuery] UsersFilter? filters = null)
    {
        return usersRepository.GetUsers(filters);
    }
}
