using OneOf;
using OneOf.Types;
using Web.Hub.Core.Dtos.Messages;

namespace Web.Hub.Core.Repositories;

public interface IMessagesManagerRepository : IBaseRepository
{
    Task<MessageInfo> AddMessage(MessageCreate message);

    Task<OneOf<MessageInfo, NotFound>> UpdateMessage(MessageUpdate dialog);

    Task RemoveMessage(Guid messageId, long userId);
}