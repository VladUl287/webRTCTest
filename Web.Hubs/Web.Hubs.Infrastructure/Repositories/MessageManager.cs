using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Core.Entities;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class MessageManager : IMessageManager
{
    private readonly DatabaseContext dbContext;

    public MessageManager(DatabaseContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Guid> Add(Message message)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges()
    {
        throw new NotImplementedException();
    }
}
