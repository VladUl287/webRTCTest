using Web.Hubs.Core.Dtos;

namespace Web.Hubs.Core.Services;

public interface ICallService
{
    Task<CallDto> Get(Guid callId);

    Task<int> Count(Guid callId);

    Task Add(Guid callId, long value);

    Task<bool> Has(Guid callId, long value);

    Task<bool> HasKey(Guid callId);

    Task<bool> HasValue(long value);

    Task Delete(Guid callId);

    Task<Guid?> Delete(long value);

    Task Delete(Guid callId, long value);
}
