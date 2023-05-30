using OneOf;
using Mapster;
using OneOf.Types;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Infrastructure.Services;

public sealed class MessageService : IMessageService
{
    private readonly IUnitOfWork unitOfWork;

    public MessageService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OneOf<MessageData, Error>> CreateMessage(MessageCreate create, long userId)
    {
        //validation

        var userInChat = await unitOfWork.ChatsUsers.AnyAsync(cu => cu.ChatId == create.ChatId && cu.UserId == userId);

        if (!userInChat)
        {
            return new Error();
        }

        var message = new Message
        {
            Content = create.Content,
            ChatId = create.ChatId,
            Date = DateTime.UtcNow,
            UserId = userId,
        };

        await unitOfWork.Messages.AddAsync(message);
        await unitOfWork.SaveChangesAsync();

        return message.Adapt<MessageData>();
    }

    public async Task<OneOf<MessageData, NotFound>> DeleteMessage(Guid messageId, long userId)
    {
        //validation

        var message = await unitOfWork.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(ms => ms.Id == messageId && ms.UserId == userId);

        if (message is null)
        {
            return new NotFound();
        }

        unitOfWork.Messages.Remove(message);
        await unitOfWork.SaveChangesAsync();

        return message.Adapt<MessageData>();
    }

    public async Task<OneOf<MessageData, NotFound>> UpdateMessage(MessageUpdate update, long userId)
    {
        //validation

        var message = await unitOfWork.Messages
            .FirstOrDefaultAsync(m => m.Id == update.Id && m.UserId == userId);

        if (message is null)
        {
            return new NotFound();
        }

        message.Content = update.Content;

        await unitOfWork.SaveChangesAsync();

        return message.Adapt<MessageData>();
    }
}
