using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Filters;

namespace Web.Hubs.Core.Repositories;

public interface IChatPresenter
{
    Task<bool> ChatExists(Guid chatId);

    Task<OneOf<ChatDto, NotFound>> GetChatById(Guid chatId, long userId);

    Task<OneOf<Guid, NotFound>> GetDialogByUsers(long firstUser, long secondUser);

    Task<ChatDto[]> GetChatsForUser(long userId, PageFilter? filter = null);

    Task<long[]> GetUsersForChat(Guid chatId, PageFilter? filter = null);
}