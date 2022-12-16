using System.ComponentModel;
using System.Security.Claims;

namespace Web.Auth.Api.Extentions;

public static class ClaimsPrincipalExtensions
{
    public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal) where T : struct
    {
        if (principal is null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(loggedInUserId))
        {
            return default(T);
        }

        if (typeof(T) == typeof(Guid))
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(loggedInUserId);
        }

        return (T)Convert.ChangeType(loggedInUserId, typeof(T));
    }
}