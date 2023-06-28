using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Services;

public interface IMessageService
{
    Task<OneOf<MessageDto, Error>> Create(CreateMessageDto message, long userId);
}
