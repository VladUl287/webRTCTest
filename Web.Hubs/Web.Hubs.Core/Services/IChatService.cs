using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Results;

namespace Web.Hubs.Core.Services;

public interface IChatService
{
    Task<ChatData> CreateChat(ChatCreate chatCreate);

    Task<OneOf<Guid, AlreadyExists, Error<string>>> AddUser(Guid chatId, long creatorId, long newUserId);

    Task<OneOf<Success, NotFound>> UpdateChat(ChatUpdate chat);

    Task RemoveChat(Guid chatId, long creatorId);

    Task RemoveUsers(Guid chatId, long creatorId, long[] usersIds);
}
