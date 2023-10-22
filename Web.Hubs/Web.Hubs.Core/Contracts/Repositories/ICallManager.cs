using Web.Hubs.Core.Entities;

namespace Web.Hubs.Core.Contracts.Repositories;

public interface ICallManager : IRepository
{
    Task<Guid> Add(Call call);

    Task AddUser(CallUser callUser);

    Task Delete(Guid callId);

    Task<Guid> DeleteUser(long userId);

    Task DeleteUser(Guid callId, long userId);
}