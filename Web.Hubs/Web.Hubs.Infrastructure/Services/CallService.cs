using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Entities;
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

    public async Task Add(Guid callId, long value)
    {
        var call = await unitOfWork.Calls.FirstOrDefaultAsync(c => c.ChatId == callId);

        if (call is null)
        {
            call = new Call
            {
                ChatId = callId
            };

            await unitOfWork.Calls.AddAsync(call);
        }

        var callUser = new CallUser
        {
            CallId = call.ChatId,
            UserId = value
        };

        await unitOfWork.CallsUsers.AddAsync(callUser);
        await unitOfWork.SaveChangesAsync();
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
