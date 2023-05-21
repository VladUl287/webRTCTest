using Web.Hub.Core;
using Microsoft.EntityFrameworkCore;

namespace Web.Hub.Infrastructure.Database;

public interface IUnitOfWork
{
    DbSet<Dialog> Dialogs { get; }

    DbSet<Message> Messages { get; }

    DbSet<DialogUser> DialogsUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
