using Web.Hubs.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Web.Hubs.Infrastructure.Database;

public interface IUnitOfWork
{
    DbSet<Chat> Dialogs { get; }

    DbSet<Message> Messages { get; }

    DbSet<ChatUser> DialogsUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
