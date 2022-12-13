using System.Diagnostics;
using Web.Auth.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Web.Auth.Infrastructure.Database;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo((msg) =>
        {
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        });
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(250);
        });

        base.OnModelCreating(modelBuilder);
    }
}