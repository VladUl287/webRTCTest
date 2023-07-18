using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Enums;
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

    public ChatPresenter(DatabaseContext dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    public Task<bool> ChatExists(Guid chatId) => ChatExistsQuery(dbcontext, chatId);

    public async Task<OneOf<Guid, NotFound>> GetDialogByUsers(long firstUser, long secondUser)
    {
        var chats = await dbcontext.ChatsUsers
            .AsNoTracking()
            .Where(cu => cu.UserId == firstUser && cu.Chat!.Type == ChatType.Dialog)
            .Select(cu => cu.ChatId)
            .ToArrayAsync();

        if (chats.Length == 0)
        {
            return new NotFound();
        }

        var result = await dbcontext.ChatsUsers
            .Where(cu => cu.UserId == secondUser && chats.Contains(cu.ChatId))
            .Select(cu => cu.ChatId)
            .FirstOrDefaultAsync();

        if (result == default)
        {
            return new NotFound();
        }

        return result;
    }

    public async Task<OneOf<ChatDto, NotFound>> GetChatById(Guid chatId, long userId)
    {
        var result = await dbcontext.ChatsUsers
            .Where(cu => cu.ChatId == chatId && cu.UserId == userId)
            .Select(cu => cu.Chat!)
            .ProjectToDto(userId)
            .FirstOrDefaultAsync();

        if (result is null)
        {
            return new NotFound();
        }

        return result;
    }

    public Task<ChatDto[]> GetChatsForUser(long userId, PageFilter? filter = null)
    {
        return dbcontext.ChatsUsers
            .Where(cu => cu.UserId == userId)
            .Select(cu => cu.Chat!)
            .ProjectToDto(userId)
            .PageFilter(filter)
            .ToArrayAsync();
    }

    public Task<long[]> GetUsersForChat(Guid chatId, PageFilter? pageFilter = null)
    {
        return dbcontext.ChatsUsers
            .Where(ch => ch.ChatId == chatId)
            .Select(ch => ch.UserId)
            .PageFilter(pageFilter)
            .ToArrayAsync();
    }

    #region Compiled queries

    public static readonly Func<DatabaseContext, Guid, Task<bool>> ChatExistsQuery =
        EF.CompileAsyncQuery((DatabaseContext dbcontext, Guid chatId) =>
            dbcontext.Chats.Any(c => c.Id == chatId)
        );

    #endregion
}
