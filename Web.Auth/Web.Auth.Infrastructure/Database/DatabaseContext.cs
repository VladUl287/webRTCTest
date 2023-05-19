using System.Diagnostics;
using Web.Auth.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Web.Auth.Infrastructure.Database;

public sealed class DatabaseContext : IdentityDbContext<User>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo((msg) =>
        {
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        });

        base.OnConfiguring(optionsBuilder);
    }
}