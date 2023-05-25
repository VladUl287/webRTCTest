using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs.Api.Extensions;

internal static class ContextUser
{
    public static T GetUserId<T>(this HubCallerContext Context, IFormatProvider? formatProvider = null)
        where T : struct, IParsable<T>
    {
        if (string.IsNullOrEmpty(Context?.User?.Identity?.Name))
        {
            return default;
        }

        // var userId = Context.Claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Context.User.Identity.Name;

        if (T.TryParse(userId, formatProvider, out T result))
        {
            return result;
        }

        return default;
    }
}