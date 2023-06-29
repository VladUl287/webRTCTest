using Refit;

namespace Web.Auth.Infrastructure.Proxies;

public interface IHubApi
{
    [Get("/chats/getChatId")]
    Task<ApiResponse<Guid>> GetChatId(long userId, int chatType);
}
