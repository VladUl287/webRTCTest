using OneOf;
using Mapster;
using OneOf.Types;
using Web.Hubs.Core.Enums;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Repositories;
using Microsoft.Extensions.Logging;
using Web.Hubs.Infrastructure.Proxies;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Database.Queries;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly IAuthApi authApi;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<ChatService> logger;
    private readonly IChatPresenter chatPresenter;

    public ChatService(IUnitOfWork unitOfWork, IChatPresenter chatPresenter, IAuthApi authApi, ILogger<ChatService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.chatPresenter = chatPresenter;
        this.authApi = authApi;
        this.logger = logger;
    }

    public async Task<OneOf<Guid, Error<string>>> Create(CreateChatDto chat)
    {
        try
        {
            if (chat is { Type: ChatType.Monolog })
            {
                if (!ValidMonolog(chat))
                {
                    return new Error<string>("Not correct data");
                }

                var chatId = await ChatQueries.GetMonologId(unitOfWork.Context, chat.UserId);

                if (chatId != default)
                {
                    return chatId;
                }
            }
            else if (chat is { Type: ChatType.Dialog })
            {
                if (!ValidDialog(chat))
                {
                    return new Error<string>("Not correct data");
                }

                var firstCollocutor = chat.Users[0];
                var secondCollocutor = chat.Users[1];
                var chatId = await ChatQueries.GetDialogId(unitOfWork.Context, firstCollocutor.Id, secondCollocutor.Id);

                if (chatId != default)
                {
                    return chatId;
                }
            }

            return await CreateChat(chat);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, chat);

            return new Error<string>("Chat creation error");
        }
    }

    private async Task<Guid> CreateChat(CreateChatDto create)
    {
        var chat = create.Adapt<Chat>();

        await unitOfWork.Chats.AddAsync(chat);

        foreach (var user in create.Users)
        {
            var chatUser = new ChatUser
            {
                ChatId = chat.Id,
                UserId = user.Id
            };

            if (chat.Type is ChatType.Dialog)
            {
                var firstCollocutor = create.Users[0].Id;
                var secondCollocutor = create.Users[1].Id;

                var userInfoId = firstCollocutor;

                if (firstCollocutor == chatUser.UserId)
                {
                    firstCollocutor = secondCollocutor;
                }

                var userInfo = await authApi.GetUserInfo(userInfoId);

                chatUser.Name = userInfo.Content?.UserName ?? string.Empty;
                chatUser.Image = userInfo.Content?.Image ?? string.Empty;
            }

            await unitOfWork.ChatsUsers.AddAsync(chatUser);
        }

        await unitOfWork.SaveChangesAsync();

        return chat.Id;
    }


    public bool ValidMonolog(CreateChatDto chat)
    {
        if (chat is { Users.Length: not 1 })
        {
            return false;
        }

        var user = chat.Users[0];

        if (user is null || user.Id != chat.UserId)
        {
            return false;
        }

        return false;
    }

    public bool ValidDialog(CreateChatDto chat)
    {
        if (chat.Users is { Length: not 2 })
        {
            return false;
        }

        var firstCollocutor = chat.Users[0];
        var secondCollocutor = chat.Users[1];

        if (firstCollocutor is null || secondCollocutor is null)
        {
            return false;
        }

        if (firstCollocutor.Id == secondCollocutor.Id)
        {
            return false;
        }

        return true;
    }
}
