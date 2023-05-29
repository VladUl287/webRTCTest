using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Messages;
using Web.Hubs.Core.Services;

namespace Web.Hubs.Infrastructure.Services;

public class MessageService : IMessageService
{
    public Task<OneOf<MessageData, Error>> CreateMessage(MessageCreate message, long userId)
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<MessageData, NotFound>> DeleteMessage(Guid messageId, long userId)
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<MessageData, NotFound>> UpdateMessage(Guid messageId, long userId, string content)
    {
        throw new NotImplementedException();
    }
}
