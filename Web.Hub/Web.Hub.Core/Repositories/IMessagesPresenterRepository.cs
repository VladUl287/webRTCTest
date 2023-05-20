using Web.Hub.Core.Dtos.Messages;

namespace Web.Hub.Core.Repositories;

public interface IMessagesPresenterRepository : IBaseRepository
{
    Task<MessageInfo[]> GetMessages(Guid dialogId, long userId);
}