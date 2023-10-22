using Web.Hubs.Core.Entities;

namespace Web.Hubs.Core.Contracts.Repositories;

public interface IMessageManager : IRepository
{
    Task<Guid> Add(Message message);
}
