using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Services;

public interface IMessageService
{
    Task<MessageInfo> CreateMessage(MessageCreate message);

    Task<OneOf<MessageInfo, NotFound>> UpdateMessage(MessageUpdate chat);

    Task RemoveMessage(Guid messageId, long userId);
}
