using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.API.Helpers;
using CareGuide.API.Middlewares;
using CareGuide.Core.Interfaces;
using CareGuide.Models.DTOs.Account;
using CareGuide.Security.Interfaces;

namespace CareGuide.API.Endpoints;

public class AccountEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/accounts")
            .WithTags("Accounts")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
             .AllowAnonymous()
             .RequireRateLimiting(RateLimitingExtension.AuthPolicy)
             .WithMetadata(new IgnoreSessionMiddleware())
             .WithSummary("Create Account")
             .WithDescription("Creates a new account by creating a Person for personal information and a User for account credentials.")
             .Accepts<CreateAccountDto>("application/json")
             .Produces<AccountResponseDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
             .AllowAnonymous()
             .RequireRateLimiting(RateLimitingExtension.AuthPolicy)
             .WithMetadata(new IgnoreSessionMiddleware())
             .WithSummary("Login")
             .WithDescription("Authenticates a user and returns a JWT token and a refresh token.")
             .Accepts<LoginAccountDto>("application/json")
             .Produces<AccountResponseDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/logout", Logout)
             .WithSummary("Logout")
             .WithDescription("Logs out the current user by revoking refresh tokens and clearing cookies.")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", Refresh)
             .AllowAnonymous()
             .RequireRateLimiting(RateLimitingExtension.AuthPolicy)
             .WithMetadata(new IgnoreSessionMiddleware())
             .WithSummary("Refresh Token")
             .WithDescription("Refreshes JWT access token using a valid refresh token.")
             .Accepts<RefreshTokenDto>("application/json")
             .Produces<AccountResponseDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}/password", UpdatePassword)
             .WithSummary("Update Password")
             .WithDescription("Updates the password for a specific user account.")
             .Accepts<UpdatePasswordAccountDto>("application/json")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapDelete("/{id:guid}", DeleteAccount)
            .WithSummary("Delete Account")
            .WithDescription("Deletes a user account by its ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Create(CreateAccountDto createAccount, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var accountDto = await accountService.CreateAccountAsync(createAccount, cancellationToken);

        AuthCookieHelper.AppendRefreshToken(httpContext.Response, accountDto.RefreshToken);
        AuthCookieHelper.AppendSessionToken(httpContext.Response, accountDto.SessionToken);

        var result = new AccountResponseDto(
            accountDto.Id,
            accountDto.Email,
            accountDto.Name,
            accountDto.Gender,
            accountDto.Birthday
        );

        return Results.Created($"/accounts/{result.Id}", result);
    }

    private static async Task<IResult> Login(LoginAccountDto loginAccount, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var accountDto = await accountService.LoginAccountAsync(loginAccount, cancellationToken);

        AuthCookieHelper.AppendRefreshToken(httpContext.Response, accountDto.RefreshToken);
        AuthCookieHelper.AppendSessionToken(httpContext.Response, accountDto.SessionToken);

        var result = new AccountResponseDto(
            accountDto.Id,
            accountDto.Email,
            accountDto.Name,
            accountDto.Gender,
            accountDto.Birthday
         );

        return Results.Ok(result);
    }

    private static async Task<IResult> Logout(IUserSessionContext session, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        await accountService.LogoutAccountAsync(session.UserId, cancellationToken);

        AuthCookieHelper.RemoveRefreshToken(httpContext.Response);
        AuthCookieHelper.RemoveSessionToken(httpContext.Response);

        return Results.NoContent();
    }

    private static async Task<IResult> Refresh(RefreshTokenDto refreshRequest, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var cookieToken = httpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(cookieToken))
            return Results.Unauthorized();

        var safeRequest = refreshRequest with { RefreshToken = cookieToken };
        var accountDto = await accountService.RefreshTokenAsync(safeRequest, cancellationToken);

        AuthCookieHelper.AppendRefreshToken(httpContext.Response, accountDto.RefreshToken);
        AuthCookieHelper.AppendSessionToken(httpContext.Response, accountDto.SessionToken);

        var result = new AccountResponseDto(
            accountDto.Id,
            accountDto.Email,
            accountDto.Name,
            accountDto.Gender,
            accountDto.Birthday
         );

        return Results.Ok(result);
    }

    private static async Task<IResult> UpdatePassword(Guid id, UpdatePasswordAccountDto user, IAccountService accountService, CancellationToken cancellationToken)
    {
        await accountService.UpdatePasswordAccountAsync(id, user, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteAccount(Guid id, IAccountService accountService, CancellationToken cancellationToken)
    {
        await accountService.DeleteAccountAsync(id, cancellationToken);
        return Results.NoContent();
    }
}
