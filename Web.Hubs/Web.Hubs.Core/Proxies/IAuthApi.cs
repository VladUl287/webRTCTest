using Refit;
using Web.Hubs.Core.Dtos;

namespace Web.Hubs.Core.Proxies;

public interface IAuthApi
{
    [Get("/users/getUserInfo")]
    Task<ApiResponse<UserDto>> GetUserInfo(long userId);
}
