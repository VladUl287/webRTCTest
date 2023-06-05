﻿using Web.Hubs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database.Configuration;

namespace Web.Hubs.Infrastructure.Database;

public sealed class DatabaseContext : DbContext, IUnitOfWork
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ChatUser> ChatsUsers => Set<ChatUser>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new DialogConfiguration());
        builder.ApplyConfiguration(new MessageConfiguration());
        builder.ApplyConfiguration(new ChatUserConfiguration());

        base.OnModelCreating(builder);
    }
}
