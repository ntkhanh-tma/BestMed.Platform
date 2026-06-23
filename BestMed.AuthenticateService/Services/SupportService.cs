namespace BestMed.AuthenticateService.Services;

internal sealed class SupportService(
    IUserBusiness userBusiness,
    IExternalAuthProvider externalAuth,
    ILogger<SupportService> logger) : ISupportService
{
    public async Task<IResult> GetSupportHtmlAsync(CancellationToken ct)
    {
        var html = await userBusiness.GetSupportHtmlContentAsync(ct);
        return Results.Ok(new { html });
    }

    public async Task<IResult> GetSupportInfoAsync(CancellationToken ct)
    {
        var info = await userBusiness.GetSupportInfoAsync(ct);
        return Results.Ok(info);
    }

    public IResult GetServerName()
        => Results.Ok(new { serverName = Environment.MachineName });

    public async Task<IResult> OAuth2TokenAsync(IFormCollection form, CancellationToken ct)
    {
        try
        {
            var rawJson = await externalAuth.GetClientCredentialsTokenRawAsync(form, ct);
            return Results.Content(rawJson, "application/json");
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Client credentials grant failed: {StatusCode}", ex.StatusCode);

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(ex.Message);
                var error = doc.RootElement.TryGetProperty("error", out var e) ? e.GetString() : "invalid_client";
                var desc = doc.RootElement.TryGetProperty("error_description", out var d) ? d.GetString() : ex.Message;
                return Results.Problem(
                    detail: desc,
                    statusCode: StatusCodes.Status401Unauthorized,
                    extensions: new Dictionary<string, object?> { ["error"] = error });
            }
            catch
            {
                return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status401Unauthorized);
            }
        }
    }
}
