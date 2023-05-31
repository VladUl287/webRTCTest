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
    private readonly DatabaseContext context;

    public ChatPresenter(DatabaseContext context)
    {
        this.context = context;
    }

    public async Task<OneOf<ChatDto, NotFound>> GetChat(Guid chatId, long userId)
    {
        var result = await getChatQuery(context, chatId, userId);

        if (result is null)
        {
            return new NotFound();
        }

        return result;
    }

    public Task<ChatDto[]> GetChats(long userId, PageFilter? pageFilter = null)
    {
        return getChatsQuery(context, userId, pageFilter);
    }

    public Task<long[]> GetUsers(Guid chatId, PageFilter? pageFilter = null)
    {
        return getUsersQuery(context, chatId, pageFilter);
    }

    private static readonly Func<DatabaseContext, Guid, long, Task<ChatDto?>> getChatQuery =
        EF.CompileAsyncQuery((DatabaseContext context, Guid chatId, long userId) =>
            context.Chats
                .Select(chat => new ChatDto
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    Image = chat.Image,

                    Message = chat.Messages
                        .OrderByDescending(m => m.Date)
                        .Select(m => m.Content)
                        .FirstOrDefault(),

                    Unread = chat.Messages
                        .Count(msg => msg.UserId != userId && msg.Date >= chat.ChatUsers
                            .Where(ch => ch.UserId == userId)
                            .Select(m => m.LastRead)
                            .FirstOrDefault()
                        )
                })
                .FirstOrDefault(chat => chat.Id == chatId)
        );

    private static readonly Func<DatabaseContext, long, PageFilter?, Task<ChatDto[]>> getChatsQuery =
        EF.CompileAsyncQuery((DatabaseContext context, long userId, PageFilter? pageFilter) =>
            context.Chats
                .Select(chat => new ChatDto
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    Image = chat.Image,

                    Message = chat.Messages
                        .OrderByDescending(m => m.Date)
                        .Select(m => m.Content)
                        .FirstOrDefault(),

                    Unread = chat.Messages
                        .Count(msg => msg.UserId != userId && msg.Date >= chat.ChatUsers
                            .Where(ch => ch.UserId == userId)
                            .Select(m => m.LastRead)
                            .FirstOrDefault()
                        )
                })
                .PageFilter(pageFilter)
                .ToArray()
        );

    private static readonly Func<DatabaseContext, Guid, PageFilter?, Task<long[]>> getUsersQuery =
        EF.CompileAsyncQuery((DatabaseContext context, Guid chatId, PageFilter? pageFilter) =>
            context.ChatsUsers
                .Where(ch => ch.ChatId == chatId)
                .Select(ch => ch.UserId)
                .PageFilter(pageFilter)
                .ToArray()
        );
}
