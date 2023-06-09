using Web.Auth.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Web.Auth.Infrastructure.Database;

public sealed class DatabaseContext : IdentityDbContext<User, Role, long>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    { }
}