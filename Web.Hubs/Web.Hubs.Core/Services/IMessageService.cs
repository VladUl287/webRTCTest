using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Services;

public interface IMessageService
{
    Task<OneOf<MessageData, Error>> CreateMessage(MessageCreate message, long userId);

    Task<OneOf<MessageData, NotFound>> UpdateMessage(MessageUpdate update, long userId);

    Task<OneOf<MessageData, NotFound>> DeleteMessage(Guid messageId, long userId);
}
