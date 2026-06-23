namespace BestMed.ResidentService.DTOs;

/// <summary>
/// Schedule update entry for PUT /residents/scheduling.
/// Carries dose-time slots, PRN rules, patch config, and ePrescribing flags
/// exactly as the legacy RegScheduleDetailBO did.
/// </summary>
public sealed record RegScheduleDetailDto
{
    public Guid Id { get; init; }
    public Guid ResidentId { get; init; }

    // Dose time slots
    public TimeOnly? Dose1Time { get; init; }
    public TimeOnly? Dose2Time { get; init; }
    public TimeOnly? Dose3Time { get; init; }
    public TimeOnly? Dose4Time { get; init; }
    public TimeOnly? Dose5Time { get; init; }
    public TimeOnly? Dose6Time { get; init; }
    public TimeOnly? Dose7Time { get; init; }
    public TimeOnly? Dose8Time { get; init; }

    public string? Frequency { get; init; }
    public string? Indication { get; init; }
    public string? Direction { get; init; }
    public string? PackType { get; init; }

    // PRN fields
    public decimal? MinOnHand { get; init; }
    public decimal? MaxOnHand { get; init; }
    public decimal? MinDose { get; init; }
    public decimal? MaxDose { get; init; }
    public decimal? MaxFreq { get; init; }
    public decimal? MaxPer24 { get; init; }

    // Patch fields
    public int? PatchDuration { get; init; }

    // ePrescribing
    public string? DspId { get; init; }

    // Additional fields — extend as the legacy schema requires
    public Dictionary<string, object?> ExtraFields { get; init; } = [];
}

/// <summary>Packed schedule item for PUT /residents/scheduling.</summary>
public sealed record ScheduleDto
{
    public string? PackType { get; init; }
    public string? Indication { get; init; }
    public string? DateStart { get; init; }
    public string? DateStop { get; init; }
    public string? PrescribedAs { get; init; }
    public string? DateSuspend { get; init; }
    public string? Type { get; init; }
}

/// <summary>Request body for PUT /residents/scheduling.</summary>
public sealed record UpdateRegSchedulingRequest
{
    public IReadOnlyList<RegScheduleDetailDto> ScheduleUpdates { get; init; } = [];
    public IReadOnlyList<ScheduleDto> PackedData { get; init; } = [];
}

/// <summary>Request body for POST /residents/{id}/dose-signing/generate.</summary>
public sealed record GenerateDoseSigningRequest
{
    public DateTime DateTime { get; init; }
}

/// <summary>Request body for POST /residents/{id}/profile/clean.</summary>
public sealed record CleanProfileRequest
{
    public DateTime DateTime { get; init; }
    public bool IsCleaningProfile { get; init; }
    public bool IsCreateDoseSigning { get; init; }
}

/// <summary>Request body for POST /residents/{id}/link.</summary>
public sealed record LinkResidentRequest
{
    public Guid MatchedResidentId { get; init; }
    public string ConfirmText { get; init; } = string.Empty;
}

/// <summary>Request body for POST /residents/{id}/orders.</summary>
public sealed record CreateResidentOrderRequest
{
    public Guid DrugId { get; init; }
    public Guid ScheduleId { get; init; }
    public string ScheduleType { get; init; } = string.Empty;
    public string? Comment { get; init; }
}
