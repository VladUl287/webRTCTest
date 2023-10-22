using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Core.Entities;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Database.Queries;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class ChatManager : IChatManager
{
    private readonly DatabaseContext dbContext;

    public ChatManager(DatabaseContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Guid> Add(Chat chat)
    {
        ArgumentNullException.ThrowIfNull(chat);

        await dbContext.Chats.AddAsync(chat);

        return chat.Id;
    }

    public Task SetLastRead(Guid chatId, long userId, DateTime date)
    {
        return ChatUserQueries.UpdateDialogLastRead(dbContext, chatId, userId, date);
    }

    public Task<int> SaveChanges()
    {
        return dbContext.SaveChangesAsync();
    }
}
