using Web.Auth.Core.Contracts;
using Web.Auth.Core.Dtos;

namespace Web.Auth.Infrastructure.Services;

public sealed class UserService : IUserService
{
    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        var users = new List<UserDto>()
        {
            new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "ulyanovskiy.01@mail.ru",
                Name = "vlad"
            }
        };

        var enumerable = users.Select(x =>
        {
            Console.WriteLine("Select called");
            return x;
        }).ToList();

        return enumerable;
    }
}