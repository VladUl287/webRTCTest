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

    public async Task<OneOf<ChatData, NotFound>> GetChat(Guid chatId, long userId)
    {
        var result = await context.Chats
            .AsNoTracking()
            .Select(chat => new ChatData
            {
                Id = chat.Id,
                Name = chat.Name,
                Image = chat.Image,

                LastMessage = chat.Messages
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
            .FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (result is null)
        {
            return new NotFound();
        }

        return result;
    }

    public Task<ChatData[]> GetChats(long userId, PageFilter? pageFilter = null)
    {
        return context.Chats
            .AsNoTracking()
            .Select(chat => new ChatData
            {
                Id = chat.Id,
                Name = chat.Name,
                Image = chat.Image,

                LastMessage = chat.Messages
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
            .ToArrayAsync();
    }

    public Task<long[]> GetUsers(Guid chatId, PageFilter? pageFilter = null)
    {
        return context.ChatsUsers
            .Where(ch => ch.ChatId == chatId)
            .Select(ch => ch.UserId)
            .PageFilter(pageFilter)
            .ToArrayAsync();
    }
}
