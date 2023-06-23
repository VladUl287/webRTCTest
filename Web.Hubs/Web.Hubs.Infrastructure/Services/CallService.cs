using Web.Hubs.Core.Services;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Services;

public sealed class CallService : ICallService
{
    private readonly IUnitOfWork unitOfWork;

    public CallService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public Task Add(Guid callId, long value)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasValue(long value)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Has(Guid callId, long value)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasKey(Guid callId)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid callId, long value)
    {
        throw new NotImplementedException();
    }
}
