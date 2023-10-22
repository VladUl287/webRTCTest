using Web.Hubs.Core.Entities;

namespace Web.Hubs.Core.Contracts.Repositories;

public interface IChatManager : IRepository
{
    Task<Guid> Add(Chat chat);

    Task SetLastRead(Guid chatId, long userId, DateTime date);
}
