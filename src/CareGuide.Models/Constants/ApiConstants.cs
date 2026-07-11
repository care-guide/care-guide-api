namespace CareGuide.Models.Constants
{
    public static class ApiConstants
    {
        public const string API_VERSION = "v1";
        public const string API_PREFIX = "api";
    }

    public static class AuthClientConstants
    {
        /// <summary>
        /// Explicit opt-in header for native (non-browser) clients. When present with
        /// <see cref="MobileClientTypeValue"/>, auth endpoints return the session and refresh
        /// tokens in the JSON body in addition to the existing HttpOnly cookies. Absent (the
        /// default), browser behavior is unchanged: tokens are only ever delivered via cookies.
        /// </summary>
        public const string ClientTypeHeaderName = "X-Client-Type";

        public const string MobileClientTypeValue = "mobile";
    }
}
