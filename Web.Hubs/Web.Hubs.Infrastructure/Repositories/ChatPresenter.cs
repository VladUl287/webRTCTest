using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Filters;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Extensions;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class ChatPresenter : IChatPresenter
{
    private readonly DatabaseContext dbcontext;

    public ChatPresenter(DatabaseContext context)
    {
        this.dbcontext = context;
    }

    public async Task<OneOf<ChatDto, NotFound>> GetChat(Guid chatId, long userId)
    {
        var result = await getChat(dbcontext, chatId, userId);

        if (result is null)
        {
            return new NotFound();
        }

        return result;
    }

    public Task<ChatDto[]> GetChats(long userId, PageFilter? filter = null)
    {
        return dbcontext.Chats
            .ProjectToDto(userId)
            .PageFilter(filter)
            .ToArrayAsync();
    }

    public Task<long[]> GetUsers(Guid chatId, PageFilter? pageFilter = null)
    {
        return dbcontext.ChatsUsers
            .Where(ch => ch.ChatId == chatId)
            .Select(ch => ch.UserId)
            .PageFilter(pageFilter)
            .ToArrayAsync();
    }

    private static readonly Func<DatabaseContext, Guid, long, Task<ChatDto?>> getChat =
        EF.CompileAsyncQuery((DatabaseContext context, Guid chatId, long userId) =>
            context.Chats
                .ProjectToDto(userId)
                .FirstOrDefault(chat => chat.Id == chatId)
        );
}
