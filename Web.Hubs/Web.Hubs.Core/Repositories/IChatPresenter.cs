using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Repositories;

public interface IChatPresenter
{
    Task<ChatData[]> GetChats(long userId, PageFilter? filter = null);

    Task<OneOf<ChatData, NotFound>> GetChat(Guid chatId, long userId);

    Task<long[]> GetUsers(Guid chatId, PageFilter? filter = null);
}