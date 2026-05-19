using CareGuide.Security.Contexts;
using CareGuide.Security.DTOs;
using CareGuide.Security.Interfaces;
using CareGuide.Security.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CareGuide.Security
{
    public static class DependencyInjection
    {
        private const int MinSecretKeyBytes = 64; // HMACSHA512 requires at least 512 bits

        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserSessionContext, UserSessionContext>();

            services.AddOptions<SecuritySettingsDto>()
                .Bind(configuration.GetSection("SecuritySettings"))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.SecretKey), "SecuritySettings:SecretKey is required.")
                .Validate(settings => Encoding.UTF8.GetByteCount(settings.SecretKey ?? string.Empty) >= MinSecretKeyBytes,
                    $"SecuritySettings:SecretKey must be at least {MinSecretKeyBytes} bytes (256 bits).")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Issuer), "SecuritySettings:Issuer is required.")
                .ValidateOnStart();

            var secretKey = configuration["SecuritySettings:SecretKey"];
            var issuer = configuration["SecuritySettings:Issuer"];
            var audience = configuration["SecuritySettings:Audience"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("SecuritySettings:SecretKey is not configured.");

            if (Encoding.UTF8.GetByteCount(secretKey) < MinSecretKeyBytes)
                throw new InvalidOperationException($"SecuritySettings:SecretKey must be at least {MinSecretKeyBytes} bytes (256 bits).");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("SecuritySettings:Issuer is not configured.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = !string.IsNullOrWhiteSpace(audience),
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "sub"
                };

                // Support both HttpOnly cookie (browser) and Authorization header (API clients)
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
                        {
                            var token = context.HttpContext.Request.Cookies["sessionToken"];
                            if (!string.IsNullOrWhiteSpace(token))
                                context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorizationBuilder();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
