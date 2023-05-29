using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Results;
using Web.Hubs.Core.Services;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    public Task<OneOf<Guid, AlreadyExists, Error<string>>> AddUser(Guid chatId, long creatorId, long newUserId)
    {
        throw new NotImplementedException();
    }

    public Task<ChatData> CreateChat(ChatCreate chatCreate)
    {
        throw new NotImplementedException();
    }

    public Task RemoveChat(Guid chatId, long creatorId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUsers(Guid chatId, long creatorId, long[] usersIds)
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<Success, NotFound>> UpdateChat(ChatUpdate chat)
    {
        throw new NotImplementedException();
    }
}
