using OneOf;
using Mapster;
using OneOf.Types;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Web.Hubs.Infrastructure.Services;

public sealed class MessageService : IMessageService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IValidator<CreateMessageDto> validator;

    public MessageService(IUnitOfWork unitOfWork, IValidator<CreateMessageDto> validator)
    {
        this.unitOfWork = unitOfWork;
        this.validator = validator;
    }

    public async Task<OneOf<MessageDto, ValidationResult, Error<string>>> Create(CreateMessageDto messageDto, long userId)
    {
        var validation = await validator.ValidateAsync(messageDto);

        if (validation.IsValid)
        {
            var chatUserExists = await ChatUserExistsQuery(unitOfWork.Context, messageDto.ChatId, userId);
            if (!chatUserExists)
            {
                return new Error<string>("The user is not a member of this chat");
            }

            var message = new Message
            {
                Content = messageDto.Content,
                ChatId = messageDto.ChatId,
                UserId = userId,
            };

            await unitOfWork.Messages.AddAsync(message);

            // var chatUser = await unitOfWork.ChatsUsers.FirstOrDefaultAsync(cu => cu.UserId == userId && cu.ChatId == messageDto.ChatId);

            // if (chatUser is not null)
            // {
            //     chatUser.LastRead = message.Date;
            // }

            await unitOfWork.SaveChangesAsync();

            return message.Adapt<MessageDto>();
        }

        var errorMessage = validation.Errors.FirstOrDefault()?.ErrorMessage;

        return new ValidationResult(errorMessage);
    }

    #region Compiled queries

    public static readonly Func<DatabaseContext, Guid, long, Task<bool>> ChatUserExistsQuery =
        EF.CompileAsyncQuery((DatabaseContext dbcontext, Guid chatId, long userId) =>
            dbcontext.ChatsUsers.Any(cu => cu.ChatId == chatId && cu.UserId == userId)
        );

    #endregion
}
