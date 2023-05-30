using OneOf;
using Mapster;
using OneOf.Types;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly IUnitOfWork unitOfWork;

    public ChatService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateChat(ChatCreate create)
    {
        var chat = create.Adapt<Chat>();

        await unitOfWork.Chats.AddAsync(chat);

        var chatUsers = create.Users
            .Select(us => new ChatUser
            {
                UserId = us.Id,
                ChatId = chat.Id
            });

        await unitOfWork.ChatsUsers.AddRangeAsync(chatUsers);

        await unitOfWork.SaveChangesAsync();

        return chat.Id;
    }

    public async Task<OneOf<Success, NotFound>> UpdateChat(ChatUpdate update)
    {
        var chat = await unitOfWork.Chats.FindAsync(update.Id);

        if (chat is null)
        {
            return new NotFound();
        }

        // await unitOfWork.Chats
        //     .Where(chat => chat.Id == update.Id)
        //     .ExecuteUpdateAsync((chat) =>
        //         chat.SetProperty(p => p.Name, v => update.Name ?? v.Name)
        //             .SetProperty(p => p.Image, v => update.Image ?? v.Image)
        //     );

        if (!string.IsNullOrEmpty(update.Name))
        {
            chat.Name = update.Name;
        }

        if (!string.IsNullOrEmpty(update.Image))
        {
            chat.Image = update.Image;
        }

        await unitOfWork.SaveChangesAsync();

        return new Success();
    }

    public Task RemoveChat(Guid chatId, long userId)
    {
        return unitOfWork.ChatsUsers
            .Where(cu => cu.ChatId == chatId && cu.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
