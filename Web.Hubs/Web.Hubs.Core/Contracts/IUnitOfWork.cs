using Web.Hubs.Core.Contracts.Repositories;

namespace Web.Hubs.Core.Contracts;

public interface IUnitOfWork
{    
    IChatManager ChatManager { get; }

    IMessageManager MessageManager { get; }

    Task<int> SaveChanges();
}
