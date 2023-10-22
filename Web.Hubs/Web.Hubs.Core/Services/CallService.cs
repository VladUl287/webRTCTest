using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos;
using Web.Hubs.Core.Contracts.Services;
using Web.Hubs.Core.Contracts.Repositories;

namespace Web.Hubs.Core.Services;

public sealed class CallService : ICallService
{
    private readonly ICallManager callManager;
    private readonly ICallPresenter callPresenter;

    public CallService(ICallManager callManager, ICallPresenter callPresenter)
    {
        this.callManager = callManager;
        this.callPresenter = callPresenter;
    }

    public async Task<OneOf<CallDto, NotFound>> Get(Guid callId)
    {
        var call = await callPresenter.Get(callId);

        if (call is null)
        {
            return new NotFound();
        }

        return call;
    }

    public async Task<OneOf<Success, Error>> Create(long userId)
    {
        var userExists = await callPresenter.UserExists(userId);
        if (userExists)
        {
            return new Error();
        }

        var callId = await callManager.Add(new());

        await callManager.AddUser(new()
        {
            CallId = callId,
            UserId = userId
        });

        await callManager.SaveChanges();

        return new Success();
    }

    public async Task<OneOf<Success, Error, NotFound>> AddUser(Guid callId, long userId)
    {
        var callExists = await callPresenter.Exists(callId);

        if (!callExists)
        {
            return new NotFound();
        }

        var userExists = await callPresenter.UserExists(userId);

        if (userExists)
        {
            return new Error();
        }

        await callManager.AddUser(new()
        {
            CallId = callId,
            UserId = userId
        });

        await callManager.SaveChanges();

        return new Success();
    }

    public Task Delete(Guid callId, long userId)
    {
        return callManager.DeleteUser(callId, userId);
    }

    public Task<Guid> Delete(long userId)
    {
        return callManager.DeleteUser(userId);
    }

    public Task Delete(Guid callId)
    {
        return callManager.Delete(callId);
    }
}
