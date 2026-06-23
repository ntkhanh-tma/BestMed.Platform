namespace BestMed.ResidentService.DTOs;

/// <summary>Single item in the prescribers dropdown list.</summary>
public sealed record PrescriberDropdownDto
{
    public string Text { get; init; } = null!;
    public string Value { get; init; } = null!;
}
