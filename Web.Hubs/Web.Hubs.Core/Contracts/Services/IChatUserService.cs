using OneOf;
using OneOf.Types;

namespace Web.Hubs.Core.Contracts.Services;

public interface IChatUserService
{
    Task<OneOf<Success, NotFound>> Update(Guid chatId, long userId, DateTime lastRead);
}
