using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Services;

public interface IChatService
{
    Task<OneOf<Guid, Error<string>>> Create(CreateChatDto chatCreate);

    Task<OneOf<Success, NotFound>> Update(UpdateChatDto chat);

    Task Delete(Guid chatId, long userId);
}
