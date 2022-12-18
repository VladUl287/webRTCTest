using Web.Auth.Core.Dtos;

namespace Web.Auth.Core.Contracts;

public interface IUserService
{

    Task<IEnumerable<UserDto>> GetUsers();
}