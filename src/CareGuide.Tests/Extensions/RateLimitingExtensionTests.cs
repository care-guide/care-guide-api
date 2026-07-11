using System.Security.Claims;
using CareGuide.API.Extensions;
using Microsoft.AspNetCore.Http;

namespace CareGuide.Tests.Extensions
{
    public class RateLimitingExtensionTests
    {
        private const string SharedCgnatIp = "203.0.113.10";

        [Fact]
        public void ResolveIdentityPartitionKey_TwoAuthenticatedUsersBehindSameIp_ReturnDifferentKeys()
        {
            // Arrange
            var userAContext = CreateAuthenticatedContext(userId: "11111111-1111-1111-1111-111111111111", ip: SharedCgnatIp);
            var userBContext = CreateAuthenticatedContext(userId: "22222222-2222-2222-2222-222222222222", ip: SharedCgnatIp);

            // Act
            var keyA = RateLimitingExtension.ResolveIdentityPartitionKey(userAContext);
            var keyB = RateLimitingExtension.ResolveIdentityPartitionKey(userBContext);

            // Assert
            keyA.Should().NotBe(keyB);
            keyA.Should().Be("user:11111111-1111-1111-1111-111111111111");
            keyB.Should().Be("user:22222222-2222-2222-2222-222222222222");
        }

        [Fact]
        public void ResolveIdentityPartitionKey_SameAuthenticatedUserAcrossRequests_ReturnsSameKey()
        {
            // Arrange
            var firstRequest = CreateAuthenticatedContext(userId: "33333333-3333-3333-3333-333333333333", ip: "198.51.100.1");
            var secondRequest = CreateAuthenticatedContext(userId: "33333333-3333-3333-3333-333333333333", ip: "198.51.100.2");

            // Act
            var keyFirst = RateLimitingExtension.ResolveIdentityPartitionKey(firstRequest);
            var keySecond = RateLimitingExtension.ResolveIdentityPartitionKey(secondRequest);

            // Assert
            keyFirst.Should().Be(keySecond);
        }

        [Fact]
        public void ResolveIdentityPartitionKey_AnonymousRequest_FallsBackToIpPartitioning()
        {
            // Arrange
            var context = CreateAnonymousContext(ip: SharedCgnatIp);

            // Act
            var key = RateLimitingExtension.ResolveIdentityPartitionKey(context);

            // Assert
            key.Should().Be($"ip:{SharedCgnatIp}");
        }

        [Fact]
        public void ResolveIdentityPartitionKey_AnonymousRequest_NeverUsesSpoofableHeaderAsIdentity()
        {
            // Arrange: an anonymous caller cannot claim a user identity via a header
            var context = CreateAnonymousContext(ip: SharedCgnatIp);
            context.Request.Headers["X-User-Id"] = "attacker-supplied-id";

            // Act
            var key = RateLimitingExtension.ResolveIdentityPartitionKey(context);

            // Assert
            key.Should().Be($"ip:{SharedCgnatIp}");
            key.Should().NotContain("attacker-supplied-id");
        }

        [Fact]
        public void ResolveClientIp_UsesFirstAddressFromXForwardedFor()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Forwarded-For"] = "203.0.113.55, 10.0.0.1";

            // Act
            var ip = RateLimitingExtension.ResolveClientIp(context);

            // Assert
            ip.Should().Be("203.0.113.55");
        }

        [Fact]
        public void ResolveClientIp_TwoAnonymousUsersBehindSameIp_ShareTheSameKey()
        {
            // Arrange: brute-force protection must still bucket anonymous traffic by IP
            var contextA = CreateAnonymousContext(ip: SharedCgnatIp);
            var contextB = CreateAnonymousContext(ip: SharedCgnatIp);

            // Act
            var ipA = RateLimitingExtension.ResolveClientIp(contextA);
            var ipB = RateLimitingExtension.ResolveClientIp(contextB);

            // Assert
            ipA.Should().Be(ipB);
        }

        private static DefaultHttpContext CreateAuthenticatedContext(string userId, string ip)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Forwarded-For"] = ip;

            var identity = new ClaimsIdentity(
            [
                new Claim("sub", userId)
            ], authenticationType: "Bearer");

            context.User = new ClaimsPrincipal(identity);
            return context;
        }

        private static DefaultHttpContext CreateAnonymousContext(string ip)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Forwarded-For"] = ip;
            context.User = new ClaimsPrincipal(new ClaimsIdentity()); // unauthenticated identity
            return context;
        }
    }
}
