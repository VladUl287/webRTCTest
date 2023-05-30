using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Services;

public interface IChatService
{
    Task<Guid> CreateChat(ChatCreate chatCreate);

    Task<OneOf<Success, NotFound>> UpdateChat(ChatUpdate chat);

    Task RemoveChat(Guid chatId, long userId);
}
