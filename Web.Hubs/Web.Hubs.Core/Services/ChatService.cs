using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Enums;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Dtos.Chats;
using Microsoft.Extensions.Logging;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Web.Hubs.Core.Contracts.Services;
using Web.Hubs.Core.Proxies;
using Mapster;
using Web.Hubs.Core.Contracts.Repositories;

namespace Web.Hubs.Core.Services;

public sealed class ChatService : IChatService
{
    private readonly IAuthApi authApi;
    private readonly IChatManager chatManager;
    private readonly IChatPresenter chatPresenter;
    private readonly ILogger<ChatService> logger;
    private readonly IValidator<CreateChatDto> validator;

    public ChatService(ILogger<ChatService> logger, IChatManager chatManager, IChatPresenter chatPresenter, IValidator<CreateChatDto> validator)
    {
        this.logger = logger;
        //this.authApi = authApi;
        this.validator = validator;
        this.chatManager = chatManager;
        this.chatPresenter = chatPresenter;
    }

    public async Task<OneOf<Guid, ValidationResult, Error<string>>> Create(CreateChatDto chat, long userId)
    {
        try
        {
            var validation = await validator.ValidateAsync(chat);

            if (!validation.IsValid)
            {
                return new ValidationResult(validation.Errors.FirstOrDefault()?.ErrorMessage);
            }

            var usersIds = chat.Users
                .Select(u => u.Id)
                .ToArray();

            var result = await chatPresenter.GetChatId(chat.Type, usersIds);

            var chatId = Guid.Empty;

            result.Switch(
                (id) =>
                {
                    chatId = id;
                },
                async (notFound) =>
                {
                    chatId = await AddChat(chat);
                });

            return chatId;

 
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, chat);

            return new Error<string>("Error creating chat.");
        }
    }

    private async Task<Guid> AddChat(CreateChatDto create)
    {
        var chat = create.Adapt<Chat>();

        var chatUsers = new List<ChatUser>(create.Users.Length);

        foreach (var user in create.Users)
        {
            var chatUser = await CreateChatUser(create, chat.Id, user.Id);

            chatUsers.Add(chatUser);
        }

        chat.ChatUsers = chatUsers;

        await chatManager.Add(chat);

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
