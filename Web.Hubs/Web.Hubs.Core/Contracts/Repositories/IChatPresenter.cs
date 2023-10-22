using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Filters;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Enums;

namespace Web.Hubs.Core.Contracts.Repositories;

public interface IChatPresenter
{
    Task<bool> ChatExists(Guid chatId);

    Task<OneOf<Guid, NotFound>> GetChatId(ChatType type, long[] users);

    Task<OneOf<ChatUser, NotFound>> GetChatUser(Guid chatId, long userId);

    Task<OneOf<ChatDto, NotFound>> GetChatById(Guid chatId, long userId);

    Task<ChatDto[]> GetChatsForUser(long userId, PageFilter? filter = null);

    Task<long[]> GetUsersForChat(Guid chatId, PageFilter? filter = null);
}