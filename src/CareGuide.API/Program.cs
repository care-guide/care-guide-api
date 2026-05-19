using CareGuide.API.Extensions;
using CareGuide.API.Filters;
using CareGuide.API.Middlewares;
using CareGuide.Core;
using CareGuide.Infra;
using CareGuide.Models;
using CareGuide.Models.Constants;
using CareGuide.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddCoreServices()
    .AddSecurity(builder.Configuration)
    .AddModelMappings()
    .AddModelValidation()
    .AddApiRateLimiting();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(ApiConstants.API_VERSION, options =>
{
    options.AddDocumentTransformer(async (document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "CareGuide.API",
            Version = ApiConstants.API_VERSION
        };

        var authenticationSchemeProvider = context.ApplicationServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(scheme => scheme.Name == "Bearer"))
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                }
            };

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations ?? Enumerable.Empty<KeyValuePair<HttpMethod, OpenApiOperation>>()))
            {
                operation.Value.Security ??= [];

                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    });
});

builder.Services.AddTransient<ErrorHandlerMiddleware>();
builder.Services.AddTransient<SessionMiddleware>();

var app = builder.Build();

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("Bearer");
    });
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseCors("CorsPolicy");

app.UseRouting();
app.UseAuthentication();

app.UseMiddleware<SessionMiddleware>();

app.UseAuthorization();
app.UseRateLimiter();

app.MapGroup(ApiConstants.API_PREFIX)
   .RequireAuthorization()
   .RequireRateLimiting(RateLimitingExtension.GeneralPolicy)
   .MapGroup(ApiConstants.API_VERSION)
   .AddEndpointFilterFactory(ValidationFilterFactory.Create)
   .MapAllEndpoints();

app.Run();