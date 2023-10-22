using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos;

namespace Web.Hubs.Core.Contracts.Services;

public interface ICallService
{
    Task<OneOf<CallDto, NotFound>> Get(Guid userId);

    Task<OneOf<Success, Error>> Create(long userId);

    Task<OneOf<Success, Error, NotFound>> AddUser(Guid callId, long userId);

    Task Delete(Guid callId);

    Task<Guid> Delete(long userId);

    Task Delete(Guid callId, long userId);
}
