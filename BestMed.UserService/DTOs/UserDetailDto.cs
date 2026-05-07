namespace BestMed.UserService.DTOs;

/// <summary>
/// Detailed user response including addresses.
/// </summary>
public sealed record UserDetailDto
{
    public Guid Id { get; init; }
    public string ExternalId { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<UserAddressDto> Addresses { get; init; } = [];
}

public sealed record UserAddressDto
{
    public Guid Id { get; init; }
    public string Street { get; init; } = null!;
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
    public string? Country { get; init; }
    public bool IsPrimary { get; init; }
}
