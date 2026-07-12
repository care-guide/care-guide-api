using CareGuide.API.Middlewares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace CareGuide.Tests.Middlewares;

public class ErrorHandlerMiddlewareTests
{
    [Fact(DisplayName = "Generic 500 in Production uses generic detail")]
    public async Task InvokeAsync_GenericExceptionInProduction_UsesGenericDetail()
    {
        const string internalMessage = "Sensitive schema detail: relation users_password_hash does not exist.";

        var (context, problem) = await InvokeMiddlewareAsync(
            Environments.Production,
            new Exception(internalMessage));

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problem["status"]!.GetValue<int>().Should().Be(StatusCodes.Status500InternalServerError);
        problem["detail"]!.GetValue<string>().Should().Be("An internal server error occurred. Please try again later.");
        problem["detail"]!.GetValue<string>().Should().NotContain(internalMessage);
        problem.ContainsKey("stackTrace").Should().BeFalse();
        problem.ContainsKey("traceId").Should().BeFalse();
    }

    [Fact(DisplayName = "Generic 500 in Development includes message and stackTrace")]
    public async Task InvokeAsync_GenericExceptionInDevelopment_IncludesRawMessageAndStackTrace()
    {
        const string internalMessage = "Sensitive developer detail.";

        var (context, problem) = await InvokeMiddlewareAsync(
            Environments.Development,
            new Exception(internalMessage));

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problem["detail"]!.GetValue<string>().Should().Be(internalMessage);
        problem["stackTrace"]!.GetValue<string>().Should().NotBeNullOrWhiteSpace();
        problem["traceId"]!.GetValue<string>().Should().Be(context.TraceIdentifier);
    }

    [Fact(DisplayName = "DbUpdateException in Production uses generic detail")]
    public async Task InvokeAsync_DbUpdateExceptionInProduction_UsesGenericDetail()
    {
        const string internalMessage = "Sensitive database detail: duplicate key violates constraint users_email_key.";

        var (context, problem) = await InvokeMiddlewareAsync(
            Environments.Production,
            new DbUpdateException(internalMessage));

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem["status"]!.GetValue<int>().Should().Be(StatusCodes.Status400BadRequest);
        problem["detail"]!.GetValue<string>().Should().Be("The request could not be completed because the data could not be persisted.");
        problem["detail"]!.GetValue<string>().Should().NotContain(internalMessage);
        problem.ContainsKey("stackTrace").Should().BeFalse();
        problem.ContainsKey("traceId").Should().BeFalse();
    }

    [Fact(DisplayName = "DbUpdateException in Development includes message and stackTrace")]
    public async Task InvokeAsync_DbUpdateExceptionInDevelopment_IncludesRawMessageAndStackTrace()
    {
        const string internalMessage = "Sensitive database detail.";

        var (context, problem) = await InvokeMiddlewareAsync(
            Environments.Development,
            new DbUpdateException(internalMessage));

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        problem["detail"]!.GetValue<string>().Should().Be(internalMessage);
        problem["stackTrace"]!.GetValue<string>().Should().NotBeNullOrWhiteSpace();
        problem["traceId"]!.GetValue<string>().Should().Be(context.TraceIdentifier);
    }

    [Theory(DisplayName = "429 from rate limiter (general or auth policy) yields a single problem+json body with retryAfter")]
    [InlineData("general")]
    [InlineData("auth")]
    public async Task InvokeAsync_RateLimitRejection_WritesSingleProblemJsonBodyWithRetryAfter(string policyName)
    {
        _ = policyName;

        const int retryAfterSeconds = 17;

        var env = Substitute.For<IWebHostEnvironment>();
        env.EnvironmentName.Returns(Environments.Production);

        var logger = Substitute.For<ILogger<ErrorHandlerMiddleware>>();
        var middleware = new ErrorHandlerMiddleware(logger, env);

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/test";
        context.Response.Body = new MemoryStream();

        RequestDelegate next = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            ctx.Response.Headers.RetryAfter = retryAfterSeconds.ToString();
            return Task.CompletedTask;
        };

        await middleware.InvokeAsync(context, next);

        context.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        context.Response.ContentType.Should().StartWith("application/problem+json");
        context.Response.Headers.RetryAfter.ToString().Should().Be(retryAfterSeconds.ToString());

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        var problem = JsonNode.Parse(body)!.AsObject();

        problem["status"]!.GetValue<int>().Should().Be(StatusCodes.Status429TooManyRequests);
        problem["retryAfter"]!.GetValue<int>().Should().Be(retryAfterSeconds);
        problem["type"]!.GetValue<string>().Should().StartWith("https://problems-registry.smartbear.com/");
    }

    [Fact(DisplayName = "429 without Retry-After metadata still yields a single problem+json body, no retryAfter field")]
    public async Task InvokeAsync_RateLimitRejectionWithoutRetryAfter_OmitsRetryAfterExtension()
    {
        var env = Substitute.For<IWebHostEnvironment>();
        env.EnvironmentName.Returns(Environments.Production);

        var logger = Substitute.For<ILogger<ErrorHandlerMiddleware>>();
        var middleware = new ErrorHandlerMiddleware(logger, env);

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/test";
        context.Response.Body = new MemoryStream();

        RequestDelegate next = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return Task.CompletedTask;
        };

        await middleware.InvokeAsync(context, next);

        context.Response.ContentType.Should().StartWith("application/problem+json");

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();
        var problem = JsonNode.Parse(body)!.AsObject();

        problem.ContainsKey("retryAfter").Should().BeFalse();
    }

    private static async Task<(HttpContext Context, JsonObject Problem)> InvokeMiddlewareAsync(string environmentName, Exception exception)
    {
        var env = Substitute.For<IWebHostEnvironment>();
        env.EnvironmentName.Returns(environmentName);

        var logger = Substitute.For<ILogger<ErrorHandlerMiddleware>>();
        var middleware = new ErrorHandlerMiddleware(logger, env);

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/test";
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw exception;

        await middleware.InvokeAsync(context, next);

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();
        var problem = JsonNode.Parse(body)!.AsObject();

        context.Response.ContentType.Should().StartWith("application/problem+json");
        problem["type"]!.GetValue<string>().Should().StartWith("https://problems-registry.smartbear.com/");
        problem["instance"]!.GetValue<string>().Should().Be("/api/v1/test");

        return (context, problem);
    }
}
