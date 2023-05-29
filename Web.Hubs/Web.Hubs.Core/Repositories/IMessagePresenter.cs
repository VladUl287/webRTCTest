using Web.Hubs.Core.Dtos.Filters;
using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Repositories;

public interface IMessagePresenter
{
    Task<MessageData[]> GetMessages(Guid chatId, long userId, PageFilter? filters = null);
}