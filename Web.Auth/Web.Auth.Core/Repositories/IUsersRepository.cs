using Web.Auth.Core.Dtos;

namespace Web.Auth.Core.Repositories;

public interface IUsersRepository
{
    Task<UserDto[]> GetUsers(UsersFilter? filter);
}
