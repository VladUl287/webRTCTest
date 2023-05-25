using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Repositories;

public interface IMessagePresenter
{
    Task<MessageInfo[]> GetMessages(Guid chatId, long userId, PageFilter? filters = null);
}