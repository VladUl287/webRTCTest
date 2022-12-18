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

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.CanConvertFrom(typeof(string)))
        {
            return default(T);
        }

        return (T)(converter.ConvertFromString(loggedInUserId) ?? default(T));
    }
}