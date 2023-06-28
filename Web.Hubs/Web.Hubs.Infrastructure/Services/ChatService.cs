using OneOf;
using Mapster;
using OneOf.Types;
using Web.Hubs.Core.Enums;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Dtos.Chats;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<ChatService> logger;

    public ChatService(IUnitOfWork unitOfWork, ILogger<ChatService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<OneOf<Guid, Error<string>>> Create(CreateChatDto chat)
    {
        try
        {
            if (chat is null)
            {
                return new Error<string>("");
            }

            if (chat is { Type: ChatType.Monolog })
            {
                if (chat is { Users.Length: not 1 })
                {
                    return new Error<string>("");
                }

                var user = chat.Users[0];

                if (user is null || user.Id != chat.UserId)
                {
                    return new Error<string>("");
                }

                var chatId = await getMonologId(unitOfWork.Context, user.Id);

                if (chatId != default)
                {
                    return chatId;
                }
            }
            else if (chat is { Type: ChatType.Dialog })
            {
                if (chat.Users is { Length: not 2 })
                {
                    return new Error<string>("");
                }

                var firstCollocutor = chat.Users[0];
                var secondCollocutor = chat.Users[1];

                if (firstCollocutor is null || secondCollocutor is null)
                {
                    return new Error<string>("");
                }

                if (firstCollocutor.Id == secondCollocutor.Id)
                {
                    return new Error<string>("");
                }

                var chatId = await getDialogId(unitOfWork.Context, firstCollocutor.Id, secondCollocutor.Id);

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

            return new Error<string>("");
        }
    }

    private async Task<Guid> CreateChat(CreateChatDto create)
    {
        var chat = create.Adapt<Chat>();

        var chatUsers = create.Users
            .Select(user => new ChatUser
            {
                UserId = user.Id,
                ChatId = chat.Id
            });

        await unitOfWork.Chats.AddAsync(chat);
        await unitOfWork.ChatsUsers.AddRangeAsync(chatUsers);

        await unitOfWork.SaveChangesAsync();

        return chat.Id;
    }

    #region

    private static readonly Func<DatabaseContext, long, Task<Guid>> getMonologId =
        EF.CompileAsyncQuery((DatabaseContext context, long userId) =>
            context.ChatsUsers
                .Where(cu => cu.UserId == userId && cu.Chat!.Type == ChatType.Monolog)
                .Select(cu => cu.ChatId)
                .FirstOrDefault()
        );

    private static readonly Func<DatabaseContext, long, long, Task<Guid>> getDialogId =
        EF.CompileAsyncQuery((DatabaseContext context, long firstCollocutor, long secondCollocutor) =>
            context.ChatsUsers
                .Where(cu => cu.Chat!.Type == ChatType.Dialog && cu.UserId == firstCollocutor)
                .Select(cu => cu.ChatId)
                      .Intersect(
                        context.ChatsUsers
                            .Where(cu => cu.Chat!.Type == ChatType.Dialog && cu.UserId == secondCollocutor)
                            .Select(cu => cu.ChatId)
                        )
                .FirstOrDefault()
        );

    #endregion
}
