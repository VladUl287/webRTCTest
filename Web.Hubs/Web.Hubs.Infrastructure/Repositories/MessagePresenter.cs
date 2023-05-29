using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Messages;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Core.Dtos.Filters;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class MessagePresenter : IMessagePresenter
{
    private readonly IUnitOfWork unitOfWork;

    public MessagePresenter(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public Task<MessageData[]> GetMessages(Guid chatId, long userId, PageFilter? filters = null)
    {
        throw new NotImplementedException();
    }
}
