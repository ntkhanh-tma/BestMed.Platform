namespace BestMed.AuthenticateService.Services;

public interface ISupportService
{
    Task<IResult> GetSupportHtmlAsync(CancellationToken ct);
    Task<IResult> GetSupportInfoAsync(CancellationToken ct);
    IResult GetServerName();
    Task<IResult> OAuth2TokenAsync(IFormCollection form, CancellationToken ct);
}
