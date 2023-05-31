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

    public async Task<OneOf<MessageDto, Error>> Create(CreateMessageDto create, long userId)
    {
        var chatUserExists = await unitOfWork.ChatsUsers.AnyAsync(cu => cu.ChatId == create.ChatId && cu.UserId == userId);

        if (!chatUserExists)
        {
            return new Error();
        }

        var message = new Message
        {
            Content = create.Content,
            ChatId = create.ChatId,
            UserId = userId,
        };

        await unitOfWork.Messages.AddAsync(message);
        await unitOfWork.SaveChangesAsync();

        return message.Adapt<MessageDto>();
    }

    public async Task<OneOf<MessageDto, NotFound>> Update(UpdateMessageDto update, long userId)
    {
        var message = await unitOfWork.Messages
            .FirstOrDefaultAsync(m => m.Id == update.Id && m.UserId == userId);

        if (message is null)
        {
            return new NotFound();
        }

        message.Content = update.Content;
        message.Edit = true;

        await unitOfWork.SaveChangesAsync();

        return message.Adapt<MessageDto>();
    }

    public async Task<OneOf<MessageDto, NotFound>> Delete(Guid id, long userId)
    {
        var message = await unitOfWork.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(ms => ms.Id == id && ms.UserId == userId);

        if (message is null)
        {
            return new NotFound();
        }

        unitOfWork.Messages.Remove(message);
        await unitOfWork.SaveChangesAsync();

        return message.Adapt<MessageDto>();
    }
}
