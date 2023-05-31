using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Results;

namespace Web.Hubs.Core.Services;

public interface IChatService
{
    Task<OneOf<Guid, AlreadyExists>> Create(CreateChatDto chatCreate);

    Task<OneOf<Success, NotFound>> Update(UpdateChatDto chat);

    Task Delete(Guid chatId, long userId);
}
