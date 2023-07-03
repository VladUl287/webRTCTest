using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Hubs.Core.Services;
using Web.Hubs.Core.Dtos;

namespace Web.Hubs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class CallsController : ControllerBase
{
    private readonly ICallService callService;

    public CallsController(ICallService callService)
    {
        this.callService = callService;
    }

    [HttpGet]
    public Task<CallDto> GetCall([FromQuery][BindRequired] Guid callId)
    {
        return callService.Get(callId);
    }
}
