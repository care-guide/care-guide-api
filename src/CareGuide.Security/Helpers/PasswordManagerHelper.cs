using Microsoft.AspNetCore.Identity;
using Zxcvbn;

namespace CareGuide.Security.Helpers
{
    public static class PasswordManagerHelper
    {
        private static readonly PasswordHasher<object> hasher = new();
        private static readonly object HashScope = new();

        public static string HashPassword(string password)
        {
            return hasher.HashPassword(HashScope, password);
        }

        public static bool ValidatePassword(string password, string hash)
        {
            var result = hasher.VerifyHashedPassword(HashScope, hash, password);
            return result is PasswordVerificationResult.Success
                         or PasswordVerificationResult.SuccessRehashNeeded;
        }

        public static (bool IsSecure, string Feedback) CheckPassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Password cannot be empty.");
            if (!password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter.");
            if (!password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter.");
            if (!password.Any(char.IsDigit))
                errors.Add("Password must contain at least one number.");
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                errors.Add("Password must contain at least one special character.");

            if (errors.Count > 0)
                return (false, string.Join(" ", errors));

            var result = Core.EvaluatePassword(password);
            if (result.Score >= 3)
                return (true, string.Empty);

            var suggestions = result.Feedback.Suggestions != null ? string.Join(" ", result.Feedback.Suggestions) : string.Empty;
            var warning = result.Feedback.Warning ?? string.Empty;
            var message = $"{warning} {suggestions}".Trim();

            return (false, string.IsNullOrWhiteSpace(message) ? "Password is too weak." : message);
        }
    }
}
