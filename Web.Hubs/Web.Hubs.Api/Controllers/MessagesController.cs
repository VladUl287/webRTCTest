using Web.Hubs.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Dtos.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Hubs.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class MessagesController : ControllerBase
{
    private readonly IMessagePresenter messagesPresenter;

    public MessagesController(IMessagePresenter messagesPresenter)
    {
        this.messagesPresenter = messagesPresenter;
    }

    [HttpGet]
    public async Task<IEnumerable<MessageDto>> GetMessages([FromQuery][BindRequired] Guid chatId)
    {
        var userId = User.GetUserId<long>();

        return await messagesPresenter.GetMessages(chatId, userId);
    }
}
