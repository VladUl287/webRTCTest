using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Filters;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Services;

namespace Web.Hubs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class ChatsController : ControllerBase
{
    private readonly IChatService chatService;
    private readonly IChatPresenter chatPresenter;

    public ChatsController(IChatPresenter chatPresenter, IChatService chatService)
    {
        this.chatService = chatService;
        this.chatPresenter = chatPresenter;
    }

    [HttpGet]
    public async Task<IActionResult> GetDialog([FromQuery][BindRequired] long userId)
    {
        var firstUserId = User.GetUserId<long>();

        var result = await chatPresenter.GetDialogByUsers(firstUserId, userId);

        return result.Match<IActionResult>(
            id => Ok(id),
            notFound => NotFound()
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetChat([FromQuery][BindRequired] Guid chatId)
    {
        var userId = User.GetUserId<long>();

        var result = await chatPresenter.GetChatById(chatId, userId);

        return result.Match<IActionResult>(
            success => Ok(success),
            notFound => NotFound()
        );
    }

    [HttpGet]
    public async Task<IEnumerable<ChatDto>> GetChats(PageFilter? filter = null)
    {
        var userId = User.GetUserId<long>();

        return await chatPresenter.GetChatsForUser(userId, filter);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChatDto createChat)
    {
        var userId = User.GetUserId<long>();

        var result = await chatService.Create(createChat, userId);

        return result.Match<IActionResult>(
            chatId => Created(nameof(Create), chatId),
            validation => BadRequest(validation.ErrorMessage),
            error => BadRequest(error.Value)
        );
    }
}
