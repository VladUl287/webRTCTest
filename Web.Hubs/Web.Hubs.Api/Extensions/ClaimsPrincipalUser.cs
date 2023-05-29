using System.Security.Claims;

namespace Web.Hubs.Api.Extensions;

internal static class ClaimsPrincipalUser
{
    public static T GetUserId<T>(this ClaimsPrincipal? User, IFormatProvider? formatProvider = null)
        where T : struct, IParsable<T>
    {
        if (string.IsNullOrEmpty(User?.Identity?.Name))
        {
            return default;
        }

        // var userId = Context.Claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.Identity.Name;

        if (T.TryParse(userId, formatProvider, out T result))
        {
            return result;
        }

        return default;
    }
}