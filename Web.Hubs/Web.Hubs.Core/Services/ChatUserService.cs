using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Core.Contracts.Services;

namespace Web.Hubs.Core.Services;

public sealed class ChatUserService : IChatUserService
{
    private readonly IChatManager chatManager;
    private readonly IChatPresenter chatPresenter;

    public ChatUserService(IChatPresenter chatPresenter, IChatManager chatManager)
    {
        this.chatPresenter = chatPresenter;
        this.chatManager = chatManager;
    }

    public async Task<OneOf<Success, NotFound>> Update(Guid chatId, long userId, DateTime lastRead)
    {
        if (lastRead.Kind is not DateTimeKind.Utc)
        {
            lastRead = DateTime.SpecifyKind(lastRead, DateTimeKind.Utc);
        }

        var result = await chatPresenter.GetChatUser(chatId, userId);

        if (result.IsT0 && result.AsT0.LastRead < lastRead)
        {
            await chatManager.SetLastRead(chatId, userId, lastRead);

            return new Success();
        }

        return new NotFound();
    }
}
