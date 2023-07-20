using OneOf;
using Mapster;
using OneOf.Types;
using Web.Hubs.Core.Enums;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Dtos.Chats;
using Microsoft.Extensions.Logging;
using Web.Hubs.Infrastructure.Proxies;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Database.Queries;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly IAuthApi authApi;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<ChatService> logger;
    private readonly IValidator<CreateChatDto> validator;

    public ChatService(ILogger<ChatService> logger, IUnitOfWork unitOfWork, IAuthApi authApi, IValidator<CreateChatDto> validator)
    {
        this.logger = logger;
        this.authApi = authApi;
        this.validator = validator;
        this.unitOfWork = unitOfWork;
    }

    public async Task<OneOf<Guid, ValidationResult, Error<string>>> Create(CreateChatDto chat, long userId)
    {
        try
        {
            if (chat.UserId != userId)
            {
                return new Error<string>("Error creating chat");
            }

            var validation = await validator.ValidateAsync(chat);

            if (validation.IsValid)
            {
                var chatId = chat.Type switch
                {
                    ChatType.Monolog => await ChatQueries.GetMonologId(unitOfWork.Context, chat.UserId),
                    ChatType.Dialog => await ChatQueries.GetDialogId(unitOfWork.Context, chat.Users[0].Id, chat.Users[1].Id),
                    _ => default
                };

                if (chatId != default)
                {
                    return chatId;
                }

                return await AddChat(chat);
            }

            return new ValidationResult(validation.Errors[0].ErrorMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, chat);

            return new Error<string>("Error creating chat. Try later");
        }
    }

    private async Task<Guid> AddChat(CreateChatDto create, CancellationToken cancellationToken = default)
    {
        var chat = create.Adapt<Chat>();

        await unitOfWork.Chats.AddAsync(chat, cancellationToken);

        foreach (var user in create.Users)
        {
            var chatUser = await CreateChatUser(create, chat.Id, user.Id);

            await unitOfWork.ChatsUsers.AddAsync(chatUser, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return chat.Id;
    }

    private async Task<ChatUser> CreateChatUser(CreateChatDto create, Guid chatId, long userId)
    {
        var chatUser = new ChatUser
        {
            ChatId = chatId,
            UserId = userId
        };

        if (create.Type is ChatType.Dialog)
        {
            var firstUser = create.Users[0].Id;
            var secondUser = create.Users[1].Id;

            var userInfoId = firstUser != userId ? firstUser : secondUser;

            var userInfoResult = await authApi.GetUserInfo(userInfoId);

            if (userInfoResult is { Content: null } or { IsSuccessStatusCode: false })
            {
                throw new InvalidOperationException($"Cannot get user info for user with id = {userId}");
            }

            chatUser.Name = userInfoResult.Content.UserName;
            chatUser.Image = userInfoResult.Content.Image;
        }

        return chatUser;
    }
}
