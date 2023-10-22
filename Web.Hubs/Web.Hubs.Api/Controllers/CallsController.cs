using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Hubs.Core.Contracts.Services;

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
    public async Task<IActionResult> GetCall([FromQuery][BindRequired] Guid callId)
    {
        var result = await callService.Get(callId);

        return result.Match<IActionResult>(
            call => Ok(call),
            notFound => NotFound()
        );
    }
}
