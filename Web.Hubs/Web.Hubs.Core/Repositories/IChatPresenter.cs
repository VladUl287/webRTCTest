using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Filters;

namespace Web.Hubs.Core.Repositories;

public interface IChatPresenter
{
    Task<OneOf<ChatDto, NotFound>> GetChat(Guid chatId, long userId);

    Task<ChatDto[]> GetChats(long userId, PageFilter? filter = null);

    Task<long[]> GetUsers(Guid chatId, PageFilter? filter = null);
}