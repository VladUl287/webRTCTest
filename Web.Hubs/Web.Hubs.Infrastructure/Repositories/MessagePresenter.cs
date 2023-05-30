using Mapster;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Filters;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Extensions;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class MessagePresenter : IMessagePresenter
{
    private readonly DatabaseContext context;

    public MessagePresenter(DatabaseContext context)
    {
        this.context = context;
    }

    public Task<MessageData[]> GetMessages(Guid chatId, long userId, PageFilter? pageFilter = null)
    {
        return context.Messages
            .AsNoTracking()
            .ProjectToType<MessageData>()
            .PageFilter(pageFilter)
            .ToArrayAsync();
    }
}
