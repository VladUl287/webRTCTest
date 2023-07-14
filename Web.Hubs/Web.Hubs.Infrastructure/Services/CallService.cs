using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos;
using Web.Hubs.Core.Results;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Services;

public sealed class CallService : ICallService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IChatPresenter chatPresenter;

    private const int CallUsersMinCount = 2;

    public CallService(IUnitOfWork unitOfWork, IChatPresenter chatPresenter)
    {
        this.unitOfWork = unitOfWork;
        this.chatPresenter = chatPresenter;
    }

    public async Task<OneOf<CallDto, NotFound>> Get(Guid callId)
    {
        var call = await unitOfWork.Calls
            .Select(c => new CallDto
            {
                Id = c.Id,
                Users = c.CallUsers!
                    .Select(cu => cu.UserId)
                    .ToArray()
            })
            .FirstOrDefaultAsync(c => c.Id == callId);

        if (call is null)
        {
            return new NotFound();
        }

        return call;
    }

    public async Task<OneOf<Success, NotFound, AlreadyExists>> Create(Guid callId, long userId)
    {
        var chatExists = await chatPresenter.ChatExists(callId);
        if (!chatExists)
        {
            return new NotFound();
        }

        var callExists = await unitOfWork.Calls.AnyAsync(call => call.Id == callId);
        if (callExists)
        {
            return new AlreadyExists();
        }

        var userInCall = await unitOfWork.CallsUsers.AnyAsync(c => c.UserId == userId);
        if (userInCall)
        {
            return new NotFound();
        }

        await unitOfWork.Calls.AddAsync(new()
        {
            Id = callId,
        });

        await unitOfWork.CallsUsers.AddAsync(new()
        {
            CallId = callId,
            UserId = userId
        });

        await unitOfWork.SaveChangesAsync();

        return new Success();
    }

    public async Task<OneOf<Success, Error, NotFound>> Add(Guid callId, long userId)
    {
        var callExists = await unitOfWork.Calls.AnyAsync(c => c.Id == callId);
        if (!callExists)
        {
            return new NotFound();
        }

        var userInCall = await unitOfWork.CallsUsers.AnyAsync(c => c.UserId == userId);
        if (userInCall)
        {
            return new Error();
        }

        await unitOfWork.CallsUsers.AddAsync(new()
        {
            CallId = callId,
            UserId = userId
        });
        await unitOfWork.SaveChangesAsync();

        return new Success();
    }

    public async Task Delete(Guid callId, long userId)
    {
        var userInCall = await unitOfWork.CallsUsers.AnyAsync(cu => cu.CallId == callId && cu.UserId == userId);

        if (userInCall)
        {
            var usersCount = await unitOfWork.CallsUsers.CountAsync(cu => cu.CallId == callId);

            if (usersCount - 1 < CallUsersMinCount)
            {
                await unitOfWork.Calls
                    .Where(c => c.Id == callId)
                    .ExecuteDeleteAsync();
            }
            else
            {
                await unitOfWork.CallsUsers
                    .Where(cu => cu.CallId == callId && cu.UserId == userId)
                    .ExecuteDeleteAsync();
            }
        }
    }

    public async Task<Guid> Delete(long userId)
    {
        var callId = await unitOfWork.CallsUsers
            .AsNoTracking()
            .Where(cu => cu.UserId == userId)
            .Select(cu => cu.CallId)
            .FirstOrDefaultAsync();

        if (callId != default)
        {
            var usersCount = await unitOfWork.CallsUsers.CountAsync(cu => cu.CallId == callId);

            if (usersCount - 1 < CallUsersMinCount)
            {
                await unitOfWork.Calls
                    .Where(c => c.Id == callId)
                    .ExecuteDeleteAsync();
            }
            else
            {
                await unitOfWork.CallsUsers
                    .Where(cu => cu.CallId == callId && cu.UserId == userId)
                    .ExecuteDeleteAsync();
            }
        }

        return callId;
    }

    public Task Delete(Guid callId)
    {
        return unitOfWork.Calls
            .Where(c => c.Id == callId)
            .ExecuteDeleteAsync();
    }
}
