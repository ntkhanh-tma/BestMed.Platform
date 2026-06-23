using BestMed.AuthenticateService.Models;
using BestMed.AuthenticateService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.AuthenticateService.Endpoints;

public static class PasswordEndpoints
{
    public static IEndpointRouteBuilder MapPasswordEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth/password").WithTags("Password");

        group.MapPost("/change-initial",
                ([FromBody] ChangePasswordRequest req, HttpContext ctx, IPasswordService svc, CancellationToken ct)
                => svc.ChangeInitialPasswordAsync(
                    req, ctx.User,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx), ct))
            .WithName("ChangeInitialPassword")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/change-expired",
                ([FromBody] ChangePasswordRequest req, HttpContext ctx, IPasswordService svc, CancellationToken ct)
                => svc.ChangeExpiredPasswordAsync(
                    req, ctx.User,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx), ct))
            .WithName("ChangeExpiredPassword")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/reset",
                ([FromBody] RequestPasswordResetRequest req, HttpContext ctx, IPasswordService svc, CancellationToken ct)
                => svc.RequestPasswordResetAsync(
                    req, ctx.User,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    AuthHelpers.GetDeviceHash(ctx), ct))
            .WithName("RequestPasswordReset")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapGet("/change-by-token",
                ([AsParameters] TokenQuery q, HttpContext ctx, IPasswordService svc, CancellationToken ct)
                => svc.ValidateResetTokenAsync(
                    q.Token ?? string.Empty,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetDeviceHash(ctx), ct))
            .WithName("ValidateResetToken")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/change-by-token",
                ([FromBody] ChangePasswordByTokenRequest req, HttpContext ctx, IPasswordService svc, CancellationToken ct)
                => svc.ChangePasswordByTokenAsync(
                    req, ctx.User,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx), ct))
            .WithName("ChangePasswordByToken")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/reset-sms-send",
                ([FromBody] ResetPasswordSmsSendRequest req, HttpContext ctx, IPasswordService svc, CancellationToken ct)
                => svc.ResetPasswordSmsSendAsync(
                    req,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    AuthHelpers.GetDeviceHash(ctx), ct))
            .WithName("ResetPasswordSmsSend")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/reset-sms-verify",
                ([FromBody] ResetPasswordBySmsRequest req, HttpContext ctx, IPasswordService svc, CancellationToken ct)
                => svc.ResetPasswordBySmsAsync(
                    req,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    AuthHelpers.GetDeviceHash(ctx), ct))
            .WithName("ResetPasswordBySms")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapGet("/generate-code",
                ([AsParameters] GenerateCodeQuery q, IPasswordService svc, CancellationToken ct)
                => svc.GeneratePasswordResetCodeAsync(q.LoginId ?? string.Empty, ct))
            .WithName("GeneratePasswordResetCode")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        return routes;
    }

    private sealed record TokenQuery([property: FromQuery] string? Token);
    private sealed record GenerateCodeQuery([property: FromQuery] string? LoginId);
}
