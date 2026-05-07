namespace BestMed.AuthenticateService.Models;

public sealed class ConnectRequest
{
    public required string RefreshToken { get; init; }
}
