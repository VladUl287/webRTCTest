using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Enums;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Filters;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Extensions;
using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Core.Entities;
using Web.Hubs.Infrastructure.Database.Queries;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class ChatPresenter : IChatPresenter
{
    private readonly DatabaseContext dbcontext;

    public ChatPresenter(DatabaseContext dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    public Task<bool> ChatExists(Guid chatId) => ChatExistsQuery(dbcontext, chatId);

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

    public async Task<OneOf<Guid, NotFound>> GetChatId(ChatType type, long[] users)
    {
        var chatId = type switch
        {
            ChatType.Monolog => await ChatQueries.GetMonologId(dbcontext, users[0]),
            ChatType.Dialog => await ChatQueries.GetDialogId(dbcontext, users[0], users[1]),
            _ => default
        };

        return chatId;
    }

    public async Task<OneOf<ChatUser, NotFound>> GetChatUser(Guid chatId, long userId)
    {
        var result = await dbcontext.ChatsUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

        if (result is null)
        {
            return new NotFound();
        }

        return result;
    }

    #region Compiled queries

    public static readonly Func<DatabaseContext, Guid, Task<bool>> ChatExistsQuery =
        EF.CompileAsyncQuery((DatabaseContext dbcontext, Guid chatId) =>
            dbcontext.Chats.Any(c => c.Id == chatId)
        );

    #endregion
}
