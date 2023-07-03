using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Dtos;
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

    public Task<CallDto> Get(Guid callId)
    {
        return unitOfWork.Calls
            .Where(c => c.Id == callId)
            .Select(c => new CallDto
            {
                ChatId = c.Id,
                Name = "Call Name",
                Users = new long[] { 1, 2 }
            })
            .FirstOrDefaultAsync();
    }

    public async Task Add(Guid callId, long value)
    {
        var call = await unitOfWork.Calls
            .FirstOrDefaultAsync(c => c.Id == callId);

        if (call is null)
        {
            call = new()
            {
                Id = callId
            };

            await unitOfWork.Calls.AddAsync(call);
        }

        var callUser = new CallUser
        {
            CallId = call.Id,
            UserId = value
        };

        await unitOfWork.CallsUsers.AddAsync(callUser);
        await unitOfWork.SaveChangesAsync();
    }

    public Task<bool> Has(Guid callId, long value)
    {
        return unitOfWork.CallsUsers
            .AnyAsync(cu => cu.CallId == callId && cu.UserId == value);
    }

    public Task<bool> HasKey(Guid callId)
    {
        return unitOfWork.CallsUsers
            .AnyAsync(cu => cu.CallId == callId);
    }

    public Task<bool> HasValue(long value)
    {
        return unitOfWork.CallsUsers
            .AnyAsync(cu => cu.UserId == value);
    }

    public Task Delete(Guid callId, long value)
    {
        return unitOfWork.CallsUsers
            .Where(cu => cu.CallId == callId && cu.UserId == value)
            .ExecuteDeleteAsync();
    }

    public Task Delete(Guid callId)
    {
        return unitOfWork.Calls
            .Where(cu => cu.Id == callId)
            .ExecuteDeleteAsync();
    }

    public async Task<Guid?> Delete(long value)
    {
        var callUser = await unitOfWork.CallsUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(cu => cu.UserId == value);

        if (callUser is not null)
        {
            unitOfWork.CallsUsers.Remove(callUser);

            await unitOfWork.SaveChangesAsync();
        }

        return callUser?.CallId;
    }

    public Task<int> Count(Guid callId)
    {
        return unitOfWork.CallsUsers
            .CountAsync(cu => cu.CallId == callId);
    }
}
