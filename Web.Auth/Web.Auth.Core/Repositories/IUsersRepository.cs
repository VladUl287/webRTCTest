using Web.Auth.Core.Dtos;

namespace Web.Auth.Core.Repositories;

public interface IUsersRepository
{
    Task<UserDto?> GetUser(long userId);

    Task<UserDto[]> GetUsers(UsersFilter? filter = null);
}
