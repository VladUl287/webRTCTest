using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Services;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Database.Queries;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ChatUserService : IChatUserService
{
    private readonly DatabaseContext context;

    public ChatUserService(DatabaseContext context)
    {
        this.context = context;
    }

    public async Task<OneOf<Success, NotFound>> Update(Guid chatId, long userId, DateTime lastRead)
    {
        if (lastRead.Kind is not DateTimeKind.Utc)
        {
            lastRead = DateTime.SpecifyKind(lastRead, DateTimeKind.Utc);
        }

        var relationExists = await context.ChatsUsers.AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId && cu.LastRead < lastRead);

        if (relationExists)
        {
            await ChatUserQueries.UpdateDialogLastRead(context, chatId, userId, lastRead);

            return new Success();
        }

        return new NotFound();
    }
}
