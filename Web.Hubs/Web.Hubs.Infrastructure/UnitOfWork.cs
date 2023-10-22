using Web.Hubs.Core.Contracts;
using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Repositories;

namespace Web.Hubs.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext dbContext;

    public UnitOfWork(DatabaseContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IChatManager ChatManager => new ChatManager(dbContext);

    public IMessageManager MessageManager => new MessageManager(dbContext);

    public Task<int> SaveChanges()
    {
        return dbContext.SaveChangesAsync();
    }
}
