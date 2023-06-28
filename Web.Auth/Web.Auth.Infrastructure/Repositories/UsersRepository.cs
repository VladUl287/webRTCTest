using Mapster;
using Web.Auth.Core.Dtos;
using Web.Auth.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Web.Auth.Infrastructure.Database;
using Web.Auth.Infrastructure.Extensions;

namespace Web.Auth.Infrastructure.Repositories;

public sealed class UsersRepository : IUsersRepository
{
    private readonly DatabaseContext context;

    public UsersRepository(DatabaseContext context)
    {
        this.context = context;
    }

    public Task<UserDto?> GetUser(long userId)
    {
        return context.Users
            .ProjectToType<UserDto>()
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public Task<UserDto[]> GetUsers(UsersFilter? filter)
    {
        return context.Users
            .SetUsersFilters(filter)
            .ProjectToType<UserDto>()
            .ToArrayAsync();
    }
}
