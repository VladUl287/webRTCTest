using OneOf;
using Mapster;
using OneOf.Types;
using FluentValidation;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Dtos.Messages;
using System.ComponentModel.DataAnnotations;
using Web.Hubs.Core.Contracts.Services;
using Web.Hubs.Core.Contracts;
using Web.Hubs.Core.Contracts.Repositories;

namespace Web.Hubs.Core.Services;

public sealed class MessageService : IMessageService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IChatPresenter chatPresenter;
    private readonly IValidator<CreateMessageDto> validator;

    public MessageService(IUnitOfWork unitOfWork, IChatPresenter chatPresenter, IValidator<CreateMessageDto> validator)
    {
        this.unitOfWork = unitOfWork;
        this.chatPresenter = chatPresenter;
        this.validator = validator;
    }

    public async Task<OneOf<MessageDto, ValidationResult, Error<string>>> Create(CreateMessageDto messageDto, long userId)
    {
        ArgumentNullException.ThrowIfNull(messageDto);

        var validation = await validator.ValidateAsync(messageDto);

        if (validation.IsValid)
        {
            var result = await chatPresenter.GetChatUser(messageDto.ChatId, userId);

            if (result.IsT1)
            {
                return new Error<string>("The user is not a member of this chat");
            }

            var message = new Message
            {
                Content = messageDto.Content,
                ChatId = messageDto.ChatId,
                UserId = userId,
            };

            await unitOfWork.MessageManager.Add(message);

            await unitOfWork.ChatManager.SetLastRead(message.ChatId, userId, message.Date);

            await unitOfWork.SaveChanges();

            return messageDto.Adapt<MessageDto>();
        }

        return new ValidationResult(validation.Errors.FirstOrDefault()?.ErrorMessage);
    }
}
