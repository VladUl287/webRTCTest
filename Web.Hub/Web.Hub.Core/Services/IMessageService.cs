using OneOf;
using OneOf.Types;
using Web.Hub.Core.Dtos.Messages;

namespace Web.Hub.Core.Services;

public interface IMessageService
{
    Task<MessageInfo> CreateMessage(MessageCreate message);

    Task<OneOf<MessageInfo, NotFound>> UpdateMessage(MessageUpdate dialog);

    Task RemoveMessage(Guid messageId, long userId);
}
