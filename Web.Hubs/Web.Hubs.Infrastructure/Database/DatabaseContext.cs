using Web.Hubs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database.Configuration;

namespace Web.Hubs.Infrastructure.Database;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {}

    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Call> Calls => Set<Call>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<CallUser> CallsUsers => Set<CallUser>();
    public DbSet<ChatUser> ChatsUsers => Set<ChatUser>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ChatConfiguration());
        builder.ApplyConfiguration(new CallConfiguration());
        builder.ApplyConfiguration(new MessageConfiguration());
        builder.ApplyConfiguration(new ChatUserConfiguration());
        builder.ApplyConfiguration(new CallUserConfiguration());

        base.OnModelCreating(builder);
    }
}
