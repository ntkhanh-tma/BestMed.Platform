namespace BestMed.AuthenticateService.Models;

public sealed class AuthResponse
{
    public required string AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required string TokenType { get; init; }
    public int? PasswordExpiredDayLeft { get; init; }
}
