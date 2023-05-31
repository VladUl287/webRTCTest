using OneOf;
using Mapster;
using OneOf.Types;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Results;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly IUnitOfWork unitOfWork;

    public ChatService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OneOf<Guid, AlreadyExists>> Create(CreateChatDto create)
    {
        var exists = await unitOfWork.Chats.AnyAsync(chat => chat.UserId == create.UserId && chat.Name == create.Name);

        if (exists)
        {
            return new AlreadyExists();
        }

        var chat = create.Adapt<Chat>();

        await unitOfWork.Chats.AddAsync(chat);

        var chatUsers = create.Users
            .Select(user => new ChatUser
            {
                UserId = user.Id,
                ChatId = chat.Id
            });

        await unitOfWork.ChatsUsers.AddRangeAsync(chatUsers);

        await unitOfWork.SaveChangesAsync();

        return chat.Id;
    }

    public async Task<OneOf<Success, NotFound>> Update(UpdateChatDto update)
    {
        var chat = await unitOfWork.Chats.FindAsync(update.Id);

        if (chat is null)
        {
            return new NotFound();
        }

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

    public Task Delete(Guid chatId, long userId)
    {
        return unitOfWork.ChatsUsers
            .Where(cu => cu.ChatId == chatId && cu.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
