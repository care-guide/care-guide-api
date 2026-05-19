namespace CareGuide.API.Helpers
{
    public class AuthCookieHelper
    {
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
