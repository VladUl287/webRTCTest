using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Core.Entities;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class CallManager : ICallManager
{
    private readonly DatabaseContext databaseContext;

    public CallManager(DatabaseContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<Guid> Add(Call call)
    {
        var entity = await databaseContext.Calls.AddAsync(call);

        return entity.Entity.Id;
    }

    public async Task AddUser(CallUser callUser)
    {
        await databaseContext.CallsUsers.AddAsync(callUser);
    }

    public Task Delete(Guid callId)
    {
        return databaseContext.Calls
            .Where(c => c.Id == callId)
            .ExecuteDeleteAsync();
    }

    public async Task<Guid> DeleteUser(long userId)
    {
        var callUser = await databaseContext.CallsUsers
            .FirstOrDefaultAsync(cu => cu.UserId == userId);

        databaseContext.CallsUsers.Remove(callUser);

        return callUser.CallId;
    }

    public Task DeleteUser(Guid callId, long userId)
    {
        return databaseContext.CallsUsers
            .Where(cu => cu.UserId == userId && cu.CallId == callId)
            .ExecuteDeleteAsync();
    }

    public Task<int> SaveChanges()
    {
        return databaseContext.SaveChangesAsync();
    }
}
