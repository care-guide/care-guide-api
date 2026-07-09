using System.IdentityModel.Tokens.Jwt;

namespace CareGuide.Security.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, Guid? personId, bool isAdmin);
        JwtSecurityToken? ValidateToken(string token);
        string GenerateRefreshToken();
    }
}