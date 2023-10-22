using Web.Hubs.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Hubs.Core.Dtos.Filters;
using Web.Hubs.Core.Contracts.Repositories;

namespace Web.Hubs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class MessagesController : ControllerBase
{
    private readonly IMessagePresenter messagesPresenter;

    public MessagesController(IMessagePresenter messagesPresenter)
    {
        this.messagesPresenter = messagesPresenter;
    }

    [HttpGet]
    public Task<MessageDto[]> GetMessages([FromQuery][BindRequired] Guid chatId, PageFilter? filter = null)
    {
        var userId = User.GetUserId<long>();

        return messagesPresenter.GetMessages(chatId, userId, filter);
    }
}
