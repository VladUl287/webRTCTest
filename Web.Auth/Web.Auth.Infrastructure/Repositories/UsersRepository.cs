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

    public async Task<UserDto[]> GetUsers(UsersFilter? filter)
    {
        return await context.Users.AsNoTracking()
            .SetUsersFilters(filter)
            .ProjectToType<UserDto>()
            .ToArrayAsync();
    }
}
