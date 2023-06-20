using Refit;

namespace Web.Auth.Infrastructure.Services;

public interface IHubApi
{
    [Get("/chats/getChatId")]
    Task<ApiResponse<Guid>> GetChatId(long userId, int chatType);
}
