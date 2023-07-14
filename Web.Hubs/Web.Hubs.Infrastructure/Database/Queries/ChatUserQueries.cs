using Microsoft.EntityFrameworkCore;

namespace Web.Hubs.Infrastructure.Database.Queries;

public static class ChatUserQueries
{
    public static readonly Func<DatabaseContext, Guid, long, DateTime, Task> UpdateDialogLastRead =
        EF.CompileAsyncQuery((DatabaseContext context, Guid chatId, long userId, DateTime lastRead) =>
            context.ChatsUsers
                .Where(cu => cu.ChatId == chatId && cu.UserId == userId)
                .ExecuteUpdate(set =>
                    set.SetProperty(prop => prop.LastRead, value => lastRead))
        );
}