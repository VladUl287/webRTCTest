using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Repositories;

public interface IMessagesPresenterRepository
{
    Task<MessageInfo[]> GetMessages(Guid dialogId, long userId);
}