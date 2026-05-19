using CareGuide.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CareGuide.Security.Contexts
{
    public class UserSessionContext : IUserSessionContext
    {
        public Guid UserId { get; private set; }
        public Guid PersonId { get; private set; }

        public UserSessionContext(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return;

            UserId = GetGuidClaim(user.Claims, "sub");
            PersonId = GetGuidClaim(user.Claims, "personId");
        }

        private static Guid GetGuidClaim(IEnumerable<Claim> claims, string type)
        {
            var value = claims.FirstOrDefault(c => c.Type == type)?.Value;
            return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
        }
    }
}
