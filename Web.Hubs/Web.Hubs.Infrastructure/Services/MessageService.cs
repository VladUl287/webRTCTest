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
}
