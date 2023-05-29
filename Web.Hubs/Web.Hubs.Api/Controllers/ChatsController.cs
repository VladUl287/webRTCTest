using Microsoft.AspNetCore.Mvc;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Repositories;

namespace Web.Hubs.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class ChatsController : ControllerBase
{
    private readonly IChatPresenter chatPresenter;

    public ChatsController(IChatPresenter chatPresenter)
    {
        this.chatPresenter = chatPresenter;
    }

    [HttpGet]
    public async Task<IActionResult> GetChat(Guid chatId)
    {
        var userId = User.GetUserId<long>();

        var result = await chatPresenter.GetChat(chatId, userId);

        return result.Match<IActionResult>(
            success => Ok(success),
            notFound => NotFound()
        );
    }

    [HttpGet]
    public async Task<IEnumerable<ChatData>> GetChats()
    {
        var userId = User.GetUserId<long>();

        return await chatPresenter.GetChats(userId);
    }
}
