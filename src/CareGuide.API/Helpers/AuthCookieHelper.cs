using CareGuide.Models.Constants;

namespace CareGuide.API.Helpers
{
    public class AuthCookieHelper
    {
        /// <summary>
        /// Explicit, backward-compatible signal for non-browser clients. Only returns true when the
        /// caller sends <c>X-Client-Type: mobile</c>; any other value or its absence is treated as a
        /// browser, preserving the cookie-only default. Comparison is case-insensitive.
        /// </summary>
        public static bool IsNativeClient(HttpRequest request)
        {
            return request.Headers.TryGetValue(AuthClientConstants.ClientTypeHeaderName, out var value)
                && string.Equals(value, AuthClientConstants.MobileClientTypeValue, StringComparison.OrdinalIgnoreCase);
        }

        public static void AppendRefreshToken(HttpResponse response, string refreshToken, int days = 7)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(days)
            };

            response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        public static void AppendSessionToken(HttpResponse response, string accessToken, int minutes = 15)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(minutes)
            };

            response.Cookies.Append("sessionToken", accessToken, cookieOptions);
        }

        public static void RemoveRefreshToken(HttpResponse response)
        {
            response.Cookies.Delete("refreshToken");
        }

        public static void RemoveSessionToken(HttpResponse response)
        {
            response.Cookies.Delete("sessionToken");
        }
    }
}
