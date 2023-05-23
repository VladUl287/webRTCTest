using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Repositories;

public interface IChatPresenterRepository
{
    Task<ChatData[]> GetChats(long userId);

    Task<long[]> GetUsersIds(Guid chatId);

    Task<ChatInfo> GetChatInfo(Guid dialogId, long userId);
}