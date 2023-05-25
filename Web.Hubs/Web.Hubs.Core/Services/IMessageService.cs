using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Services;

public interface IMessageService
{
    Task<OneOf<MessageInfo, Error>> CreateMessage(MessageCreate message, long userId);

    Task<OneOf<MessageInfo, NotFound>> UpdateMessage(Guid messageId, long userId, string content);

    Task<OneOf<MessageInfo, NotFound>> DeleteMessage(Guid messageId, long userId);
}
