using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Core.Dtos;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class CallPresenter : ICallPresenter
{
    private readonly DatabaseContext databaseContext;

    public CallPresenter(DatabaseContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public Task<bool> Exists(Guid callId)
    {
        return databaseContext.Calls.AnyAsync(c => c.Id == callId);
    }

    public Task<CallDto?> Get(Guid callId)
    {
        return databaseContext.Calls
            .Select(c => new CallDto
            {
                Id = callId,
                Users = c.CallUsers!
                    .Select(cu => cu.UserId)
                    .ToArray()
            })
            .FirstOrDefaultAsync(c => c.Id == callId);
    }

    public Task<bool> UserExists(long userId)
    {
        return databaseContext.CallsUsers.AnyAsync(cu => cu.UserId == userId);
    }
}
