using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Dtos;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Services;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Services;

public sealed class CallService : ICallService
{
    private readonly IUnitOfWork unitOfWork;

    private const int CallUsersMinCount = 2;

    public CallService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public Task<CallDto?> Get(Guid callId)
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

    public async Task Delete(Guid callId, long value)
    {
        var callUser = await unitOfWork.CallsUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(cu => cu.CallId == callId && cu.UserId == value);

        if (callUser is not null)
        {
            using var transaction = await unitOfWork.Context.Database.BeginTransactionAsync();

            unitOfWork.CallsUsers.Remove(callUser);

            await unitOfWork.SaveChangesAsync();

            var callUsersCount = await unitOfWork.CallsUsers.CountAsync(cu => cu.CallId == callUser.CallId);

            if (callUsersCount < CallUsersMinCount)
            {
                await unitOfWork.Calls
                    .Where(c => c.Id == callUser.CallId)
                    .ExecuteDeleteAsync();
            }

            await transaction.CommitAsync();
        }
    }

    public async Task<Guid?> Delete(long value)
    {
        var callUser = await unitOfWork.CallsUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(cu => cu.UserId == value);

        if (callUser is not null)
        {
            using var transaction = await unitOfWork.Context.Database.BeginTransactionAsync();

            unitOfWork.CallsUsers.Remove(callUser);

            await unitOfWork.SaveChangesAsync();

            var callUsersCount = await unitOfWork.CallsUsers.CountAsync(cu => cu.CallId == callUser.CallId);

            if (callUsersCount < CallUsersMinCount)
            {
                await unitOfWork.Calls
                    .Where(c => c.Id == callUser.CallId)
                    .ExecuteDeleteAsync();
            }

            await transaction.CommitAsync();
        }

        return callUser?.CallId;
    }

    public Task Delete(Guid callId)
    {
        return unitOfWork.Calls
            .Where(c => c.Id == callId)
            .ExecuteDeleteAsync();
    }
}
