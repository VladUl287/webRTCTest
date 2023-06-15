﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Filters;
using Web.Hubs.Core.Repositories;

namespace Web.Hubs.Api.Controllers;

[Authorize]
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
    public async Task<IActionResult> GetChat([FromQuery][BindRequired] Guid chatId)
    {
        var userId = User.GetUserId<long>();

        var result = await chatPresenter.GetChat(chatId, userId);

        return result.Match<IActionResult>(
            success => Ok(success),
            notFound => NotFound()
        );
    }

    [HttpGet]
    public async Task<IEnumerable<ChatDto>> GetChats(PageFilter? filter = null)
    {
        var userId = User.GetUserId<long>();

        return await chatPresenter.GetChats(userId, filter);
    }
}
