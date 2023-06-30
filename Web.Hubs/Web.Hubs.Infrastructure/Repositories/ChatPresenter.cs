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

    public ChatPresenter(DatabaseContext context)
    {
        this.dbcontext = context;
    }

    public async Task<OneOf<ChatDto, NotFound>> GetChat(Guid chatId, long userId)
    {
        // var chatUser = await dbcontext.ChatsUsers
        //     .AsNoTracking()
        //     .FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

        // if (chatUser is null)
        // {
        //     return new NotFound();
        // }

        // var chat = await dbcontext.Chats
        //     .AsNoTracking()
        //     .FirstOrDefaultAsync(c => c.Id == chatId);

        // if (chat is null)
        // {
        //     return new NotFound();
        // }

        // var result = new ChatDto
        // {
        //     Id = chat.Id,
        //     Date = chat.Date,
        //     Name = chat.Name,
        //     Image = chat.Image,
        //     UserId = chat.UserId,
        //     LastRead = chatUser.LastRead,
        //     Unread = await dbcontext.Messages
        //         .CountAsync(m => m.UserId != userId && m.Date > chatUser.LastRead),

        //     LastMessage = await dbcontext.Messages
        //         .Where(m => m.ChatId == chat.Id)
        //         .OrderByDescending(m => m.Date)
        //         .Select(m => new LastMessageDto
        //         {
        //             Content = m.Content,
        //             Date = m.Date
        //         })
        //         .FirstOrDefaultAsync()
        // };

        // if (chat.Type is ChatType.Dialog)
        // {
        //     chat.Name = chatUser.Name;
        //     chat.Image = chatUser.Image;
        // }

        // return result;

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

    public Task<ChatDto[]> GetChats(long userId, PageFilter? filter = null)
    {
        return dbcontext.ChatsUsers
            .Where(cu => cu.UserId == userId)
            .Select(cu => cu.Chat!)
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

    public async Task<OneOf<Guid, NotFound>> GetChatId(long firstUser, long secondUser, ChatType chatType)
    {
        var chats = await dbcontext.ChatsUsers
            .Where(cu => cu.UserId == firstUser && cu.Chat!.Type == chatType)
            .Select(cu => cu.ChatId)
            .ToListAsync();

        if (chats.Count == 0)
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

    private static readonly Func<DatabaseContext, Guid, long, Task<ChatDto?>> getChat =
        EF.CompileAsyncQuery((DatabaseContext context, Guid chatId, long userId) =>
            context.Chats
                .ProjectToDto(userId)
                .FirstOrDefault(chat => chat.Id == chatId)
        );
}
