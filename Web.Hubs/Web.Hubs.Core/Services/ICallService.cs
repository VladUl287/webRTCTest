using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos;
using Web.Hubs.Core.Results;

namespace Web.Hubs.Core.Services;

public interface ICallService
{
    Task<OneOf<CallDto, NotFound>> Get(Guid userId);

    Task<OneOf<Success, NotFound, AlreadyExists>> Create(Guid callId, long userId);

    Task<OneOf<Success, Error, NotFound>> Add(Guid callId, long userId);

    Task Delete(Guid callId);

    Task<Guid> Delete(long userId);

    Task Delete(Guid callId, long userId);
}
