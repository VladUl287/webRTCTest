using Web.Hub.Core;
using Microsoft.EntityFrameworkCore;
using Web.Hub.Infrastructure.Database.Configuration;

namespace Web.Hub.Infrastructure.Database;

public sealed class DatabaseContext : DbContext, IUnitOfWork
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    { }

    public DbSet<Dialog> Dialogs => Set<Dialog>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<DialogUser> DialogsUsers => Set<DialogUser>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new DialogConfiguration());
        builder.ApplyConfiguration(new MessageConfiguration());
        builder.ApplyConfiguration(new DialogUserConfiguration());

        base.OnModelCreating(builder);
    }
}
