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

    public async Task<OneOf<Guid, Error<string>>> Create(CreateChatDto chatDto)
    {
        try
        {
            var monologResult = await ValidateMonolog(chatDto);
            if (monologResult.IsT0)
            {
                return new Error<string>(monologResult.AsT0);
            }

            var dialogResult = await ValidateDialog(chatDto);
            if (dialogResult.IsT0)
            {
                return new Error<string>(dialogResult.AsT0);
            }

            if (dialogResult.IsT1)
            {
                return dialogResult.AsT1;
            }

            return await CreateChat(chatDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, chatDto);

            return new Error<string>("");
        }
    }

    public async Task<OneOf<Success, NotFound>> Update(UpdateChatDto update)
    {
        if (string.IsNullOrEmpty(update.Name) || string.IsNullOrEmpty(update.Image))
        {
            return new Success();
        }

        var chat = await unitOfWork.Chats.FindAsync(update.Id);

        if (chat is null)
        {
            return new NotFound();
        }

        chat.Name = update.Name;
        chat.Image = update.Image;

        await unitOfWork.SaveChangesAsync();

        return new Success();
    }

    public Task Delete(Guid chatId, long userId)
    {
        return unitOfWork.ChatsUsers
            .Where(cu => cu.ChatId == chatId && cu.UserId == userId)
            .ExecuteDeleteAsync();
    }

    #region 

    private static readonly Func<DatabaseContext, long, ChatType, Task<bool>> chatExists =
        EF.CompileAsyncQuery((DatabaseContext context, long userId, ChatType type) =>
            context.Chats.Any(chat => chat.UserId == userId && chat.Type == type)
        );

    private async Task<OneOf<string, None>> ValidateMonolog(CreateChatDto chatDto)
    {
        if (chatDto is { Type: ChatType.Monolog })
        {
            if (chatDto is { Users.Length: not 1 })
            {
                return "";
            }

            var user = chatDto.Users[0];
            if (user.Id != chatDto.UserId)
            {
                return "";
            }

            var exists = await chatExists(unitOfWork.Context, chatDto.UserId, chatDto.Type);
            if (exists)
            {
                return "";
            }
        }

        return new None();
    }

    private async Task<OneOf<string, Guid, None>> ValidateDialog(CreateChatDto chatDto)
    {
        if (chatDto is { Type: ChatType.Dialog })
        {
            if (chatDto.Users is { Length: not 2 })
            {
                return "";
            }

            if (!chatDto.Users.Any(user => user.Id == chatDto.UserId))
            {
                return "";
            }

            var secondUser = chatDto.Users.FirstOrDefault(u => u.Id != chatDto.UserId);
            if (secondUser is null)
            {
                return "";
            }

            var chats = await unitOfWork.ChatsUsers
                .AsNoTracking()
                .Where(cu => cu.UserId == chatDto.UserId && cu.Chat!.Type == ChatType.Dialog)
                .Select(cu => cu.ChatId)
                .ToListAsync();

            var chatId = await unitOfWork.ChatsUsers
                .Where(cu => cu.UserId == secondUser.Id && chats.Contains(cu.ChatId))
                .Select(cu => cu.ChatId)
                .FirstOrDefaultAsync();

            if (chatId != default)
            {
                return chatId;
            }
        }

        return new None();
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

    #endregion
}
