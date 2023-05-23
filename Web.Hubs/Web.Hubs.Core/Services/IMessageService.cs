using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Services;

public interface IMessageService
{
    Task<OneOf<MessageInfo, Error>> CreateMessage(MessageCreate message);

    Task<OneOf<MessageInfo, NotFound>> UpdateMessage(Guid messageId, long userId, string content);

    Task<OneOf<Success, NotFound>> DeleteMessage(Guid messageId, long userId);
}
