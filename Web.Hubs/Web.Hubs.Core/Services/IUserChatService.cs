using OneOf;
using OneOf.Types;

namespace Web.Hubs.Core.Services;

public interface IUserChatService
{
    Task<OneOf<Success, Error, NotFound>> Update(Guid chatId, long userId, DateTime lastRead);
}
