using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using Web.Hubs.Core;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Repositories;

public sealed class ChatPresenter : IChatPresenter
{
    private readonly IUnitOfWork unitOfWork;

    public ChatPresenter(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OneOf<ChatData, NotFound>> GetChat(Guid chatId, long userId)
    {
        var result = await unitOfWork.Chats
            .AsNoTracking()
            .Where(x => x.Id == chatId && x.UserId == userId)
            .ProjectToType<ChatData>()
            .FirstOrDefaultAsync();

        if (result is null)
        {
            return new NotFound();
        }

        return result;
    }

    public async Task<ChatData[]> GetChats(long userId, PageFilter? pageFilter = null)
    {
        var result = await unitOfWork.Chats
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ProjectToType<ChatData>()
            .ToArrayAsync();

        return result;
    }

    public Task<long[]> GetUsers(Guid chatId, PageFilter? filter = null)
    {
        var users = unitOfWork.ChatsUsers
            .Where(x => x.ChatId == chatId)
            .Select(x => x.UserId);

        if (filter is not null)
        {
            var skip = (filter.Page - 1) * filter.Size;
            var take = filter.Size;

            users = users.Skip(skip).Take(take);
        }

        return users.ToArrayAsync();
    }
}
