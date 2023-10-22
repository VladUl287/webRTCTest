using Web.Hubs.Core.Dtos;

namespace Web.Hubs.Core.Contracts.Repositories;

public interface ICallPresenter
{
    Task<CallDto?> Get(Guid callId);

    Task<bool> Exists(Guid callId);

    Task<bool> UserExists(long userId);
}
