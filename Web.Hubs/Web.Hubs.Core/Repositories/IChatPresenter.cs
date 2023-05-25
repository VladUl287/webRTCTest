using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Repositories;

public interface IChatPresenter
{
    Task<ChatData[]> GetChats(long userId, PageFilter? pageFilter = null);

    Task<ChatData> GetChat(Guid chatId, long userId);

    Task<long[]> GetUsers(Guid chatId, PageFilter? pageFilter = null);
}