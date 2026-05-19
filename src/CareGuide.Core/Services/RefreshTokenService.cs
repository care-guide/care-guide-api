using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;

namespace CareGuide.Core.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IJwtService jwtService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
        }

        public async Task<RefreshToken> CreateAsync(Guid userId, CancellationToken cancellationToken)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = _jwtService.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Revoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            return refreshToken;
        }

        public async Task<RefreshToken?> GetAsync(Guid userId, string token, CancellationToken cancellationToken)
        {
            return await _refreshTokenRepository.GetByTokenAsync(userId, token, cancellationToken);
        }

        public async Task<RefreshToken> RotateAsync(Guid userId, string oldToken, CancellationToken cancellationToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(userId, oldToken, cancellationToken);

            if (storedToken == null || storedToken.ExpiresAt <= DateTime.UtcNow || storedToken.Revoked)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            storedToken.Revoked = true;

            var newToken = new RefreshToken
            {
                UserId = userId,
                Token = _jwtService.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Revoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.InvalidateAndReplaceAsync(storedToken, newToken, cancellationToken);

            return newToken;
        }

        public async Task InvalidateAsync(Guid userId, string token, CancellationToken cancellationToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(userId, token, cancellationToken);
            if (storedToken != null)
            {
                storedToken.Revoked = true;
                await _refreshTokenRepository.InvalidateAndReplaceAsync(storedToken, storedToken, cancellationToken);
            }
        }

        public async Task InvalidateAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userTokens = await _refreshTokenRepository.GetAllByUserAsync(userId, cancellationToken);

            foreach (var token in userTokens)
                token.Revoked = true;

            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
