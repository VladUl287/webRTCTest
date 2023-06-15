using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Services;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Services;

public sealed class UserChatService : IUserChatService
{
    private readonly DatabaseContext context;

    public UserChatService(DatabaseContext context)
    {
        this.context = context;
    }

    public async Task<OneOf<Success, Error, NotFound>> Update(Guid chatId, long userId, DateTime lastRead)
    {
        var chatUser = await context.ChatsUsers.FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

        if (chatUser is null)
        {
            return new NotFound();
        }

        if (chatUser.LastRead > lastRead)
        {
            return new Error();
        }

        chatUser.LastRead = lastRead;

        await context.SaveChangesAsync();

        return new Success();
    }
}
