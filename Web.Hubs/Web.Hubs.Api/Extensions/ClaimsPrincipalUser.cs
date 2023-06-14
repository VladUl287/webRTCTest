using System.Security.Claims;

namespace Web.Hubs.Api.Extensions;

internal static class ClaimsPrincipalUser
{
    public static T GetUserId<T>(this ClaimsPrincipal? User, IFormatProvider? formatProvider = null)
        where T : struct, IParsable<T>
    {
        var userId = User?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return default;
        }

        if (T.TryParse(userId, formatProvider, out T result))
        {
            return result;
        }

        return default;
    }
}