using OneOf;
using OneOf.Types;
using Web.Hubs.Core;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Core.Dtos.Filters;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class ChatPresenter : IChatPresenter
{
    private readonly DatabaseContext context;

    // private Func<DatabaseContext, Guid, long, ChatData?> GetChatQuery =
    //     EF.CompileQuery((DatabaseContext context, Guid chatId, long userId) => context.Chats
    //         .Select(chat => new ChatData
    //         {
    //             Id = chat.Id,
    //             Name = chat.Name,
    //             Image = chat.Image,

    //             LastMessage = chat.Messages
    //                 .OrderByDescending(m => m.Date)
    //                 .Select(m => m.Content)
    //                 .FirstOrDefault(),

    //             Unread = chat.Messages
    //                 .Count(msg => msg.UserId != userId && msg.Date >= chat.ChatUsers
    //                     .Where(ch => ch.UserId == userId)
    //                     .Select(m => m.LastRead)
    //                     .FirstOrDefault()
    //                 )
    //         })
    //         .FirstOrDefault(chat => chat.Id == chatId)
    //     );

    public ChatPresenter(DatabaseContext context)
    {
        this.context = context;
    }

    public Task<OneOf<ChatData, NotFound>> GetChat(Guid chatId, long userId)
    {
        throw new NotImplementedException();
    }

    public Task<ChatData[]> GetChats(long userId, PageFilter? filter = null)
    {
        throw new NotImplementedException();
    }

    public Task<long[]> GetUsers(Guid chatId, PageFilter? filter = null)
    {
        throw new NotImplementedException();
    }
}
