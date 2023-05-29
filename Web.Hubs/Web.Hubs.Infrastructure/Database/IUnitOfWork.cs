using Web.Hubs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Web.Hubs.Infrastructure.Database;

public interface IUnitOfWork
{
    DatabaseFacade Database { get; }

    DbSet<Chat> Chats { get; }

    DbSet<Message> Messages { get; }

    DbSet<ChatUser> ChatsUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
