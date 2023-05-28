using Web.Hubs.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Web.Hubs.Infrastructure.Database;

public interface IUnitOfWork
{
    DbSet<Chat> Chats { get; }

    DbSet<Message> Messages { get; }

    DbSet<ChatUser> ChatsUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
