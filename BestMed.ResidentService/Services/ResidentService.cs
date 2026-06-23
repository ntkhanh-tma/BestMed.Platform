using System.Security.Claims;
using BestMed.Common.Models;
using BestMed.ResidentService.Data;
using BestMed.ResidentService.DTOs;
using BestMed.ResidentService.Mapping;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.ResidentService.Services;

/// <summary>
/// Core resident business logic. Owns everything that IResidentBusiness did in the monolith.
/// Uses ReadOnlyResidentDbContext for all queries and ResidentDbContext for writes.
/// Cross-service calls are delegated to IFacilityClient; other external domains are stubs (see TODO).
/// </summary>
public sealed class ResidentService(
    ResidentDbContext db,
    ReadOnlyResidentDbContext readDb,
    IOutputCacheStore cache,
    IFacilityClient facilityClient,
    ILogger<ResidentService> logger) : IResidentService
{
    private const string CacheTag = "residents";

    // ── List / Search ──────────────────────────────────────────────────────────

    public async Task<IResult> GetResidentsAsync(ResidentQueryParameters query, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = readDb.Residents
                .ApplyFilters(query)
                .ApplySorting(query);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => r.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(new PagedResponse<ResidentDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying residents");
            return Results.Problem("An error occurred while querying residents.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> QuickSearchAsync(QuickSearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var terms = request.SearchTerms?.Trim() ?? string.Empty;

            var query = readDb.Residents.AsQueryable();

            if (!string.IsNullOrEmpty(terms))
            {
                query = query.Where(r =>
                    (r.FirstName != null && r.FirstName.Contains(terms)) ||
                    (r.LastName != null && r.LastName.Contains(terms)) ||
                    (r.DisplayName != null && r.DisplayName.Contains(terms)) ||
                    (r.FredCode != null && r.FredCode.Contains(terms)));
            }

            if (request.PharmacyId.HasValue)
            {
                // TODO: Filter by pharmacy once PharmacyId is reachable from resident context
                //       (e.g. via FacilityId → Facility.PharmacyId join or a denormalised column).
            }

            if (request.WarehouseId.HasValue)
            {
                // TODO: Filter by warehouse when the resident→pharmacy→warehouse relationship is mapped.
            }

            var results = await query
                .Take(50)
                .Select(r => r.ToQuickSearchDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error performing quick search for residents");
            return Results.Problem("An error occurred during resident quick search.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetResidentListAsync(Guid? pharmacyId, Guid? facilityId, Guid? sectionId, Guid? prescriberId, CancellationToken cancellationToken)
    {
        try
        {
            var query = readDb.Residents.AsQueryable();

            if (facilityId.HasValue)
                query = query.Where(r => r.FacilityId == facilityId.Value);

            if (sectionId.HasValue)
                query = query.Where(r => r.SectionId == sectionId.Value);

            // TODO: prescriberId filter requires a resident→prescriber relationship table.
            // TODO: pharmacyId filter requires a facility→pharmacy join or a denormalised field.

            var items = await query
                .OrderBy(r => r.LastName)
                .ThenBy(r => r.FirstName)
                .Select(r => r.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(items);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching resident list");
            return Results.Problem("An error occurred while fetching the resident list.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetResidentDetailsAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            var resident = await readDb.Residents
                .FirstOrDefaultAsync(r => r.Id == residentId, cancellationToken);

            return resident is null
                ? Results.NotFound()
                : Results.Ok(resident.ToDetailDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving resident {ResidentId}", residentId);
            return Results.Problem("An error occurred while retrieving the resident.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetFacilityNameAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            var facilityId = await readDb.Residents
                .Where(r => r.Id == residentId)
                .Select(r => (Guid?)r.FacilityId)
                .FirstOrDefaultAsync(cancellationToken);

            // TODO: Return the actual facility name by calling FacilityService
            //       once FacilityService exposes GET /facilities/{id}/name or similar.
            //       For now return facilityId so the caller can resolve the name independently.
            return facilityId is null
                ? Results.NotFound()
                : Results.Ok(new { FacilityId = facilityId, FacilityName = (string?)null });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching facility name for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred while fetching the facility name.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetMedicationTrackingResidentsAsync(IReadOnlyList<Guid>? facilityIds, CancellationToken cancellationToken)
    {
        try
        {
            var query = readDb.Residents.AsQueryable();

            if (facilityIds is { Count: > 0 })
                query = query.Where(r => facilityIds.Contains(r.FacilityId));

            var items = await query
                .OrderBy(r => r.LastName)
                .Select(r => r.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(items);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching medication tracking residents");
            return Results.Problem("An error occurred while fetching residents for medication tracking.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Profile ────────────────────────────────────────────────────────────────

    public async Task<IResult> GetProfileAsync(Guid residentId, Guid? drugId, string defaultTab, CancellationToken cancellationToken)
    {
        try
        {
            var resident = await readDb.Residents
                .FirstOrDefaultAsync(r => r.Id == residentId, cancellationToken);

            if (resident is null)
                return Results.NotFound();

            // TODO: shouldShowWarfarinPopup = await drugClient.IsMainVariableDrugAsync(drugId, ct)
            //       when IDrugClient is implemented.
            var shouldShowWarfarinPopup = false;

            // TODO: isEnableMedicationTracking = await medicationTrackingClient.IsShowMedicationTrackingForResidentAsync(residentId, ct)
            //       when IMedicationTrackingClient is implemented.
            var isEnableMedicationTracking = false;

            return Results.Ok(new ResidentProfileDto
            {
                Id = resident.Id,
                Status = resident.Status,
                IsOtherSupplyPharmacy = resident.IsOtherSupplyPharmacy ?? false,
                IsRestrictedByFacilityConfig = resident.IsRestrictedByFacilityConfig ?? false,
                FacilityId = resident.FacilityId,
                HasEditRights = false,  // Resolved by the endpoint from JWT claims — injected by caller
                IsNationalPlatformUser = false, // Resolved from JWT custom claim — injected by caller
                ShouldShowWarfarinPopup = shouldShowWarfarinPopup,
                WarfarinDrugId = shouldShowWarfarinPopup ? drugId : null,
                IsEnableMedicationTracking = isEnableMedicationTracking,
                DefaultTab = string.IsNullOrEmpty(defaultTab) ? "1" : defaultTab
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving profile for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred while retrieving the resident profile.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetHeaderAsync(Guid residentId, bool allergies, bool checkIhi, CancellationToken cancellationToken)
    {
        try
        {
            var resident = await readDb.Residents
                .FirstOrDefaultAsync(r => r.Id == residentId, cancellationToken);

            if (resident is null)
                return Results.Ok(new { });

            // TODO: isEnableMedicationTracking = await medicationTrackingClient.IsShowMedicationTrackingForResidentAsync(residentId, ct)
            var isEnableMedicationTracking = false;

            // Photo timestamp: emit epoch sentinel when null (legacy contract, §6 Known Business Rules)
            var photoLastUpdate = resident.PhotoLastUpdate.HasValue
                ? resident.PhotoLastUpdate.Value.ToString("yyyy-MM-ddTHH:mm:ss")
                : "2000-01-01T00:00:00";

            return Results.Ok(new ResidentHeaderDto
            {
                Id = resident.Id,
                DisplayName = resident.DisplayName,
                FacilityId = resident.FacilityId,
                Status = resident.Status,
                PhotoLastUpdate = photoLastUpdate,
                EnableDocumentManagement = resident.EnableDocumentManagement ?? false,
                IsOtherSupplyPharmacy = resident.IsOtherSupplyPharmacy ?? false,
                HasEditRights = false,  // Resolved from JWT claims by the endpoint
                IsEnableMedicationTracking = isEnableMedicationTracking,
                // isEnableResidentDocumentManagement and ResidentMissedIHIWarning require
                // hasEditRights from JWT — endpoint sets these after resolving claims.
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving header for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred while retrieving the resident header.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetLastPackedUntilDateAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Query the PackRequest/PackResidentRoll table for the last pack-end date.
            //       PackResidentRoll lives in PharmacyService domain; coordinate ownership.
            logger.LogWarning("GetLastPackedUntilDate called for {ResidentId} — cross-domain query not yet implemented", residentId);
            return await Task.FromResult(Results.Ok((DateTime?)null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting last packed until date for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetProfileLockExpiryAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Retrieve the profile lock expiry from the MedProfile lock record.
            var expiry = await readDb.MedProfiles
                .Where(p => p.ResidentId == residentId && p.IsActive && p.LockedAt.HasValue)
                .Select(p => p.LockedAt)
                .FirstOrDefaultAsync(cancellationToken);

            return Results.Ok(new { ExpiresAt = expiry?.ToString("o") });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting profile lock expiry for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> HasForcedDeleteProfileAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            var has = await readDb.MedProfiles
                .AnyAsync(p => p.ResidentId == residentId && p.IsForcedDelete == true, cancellationToken);

            return Results.Ok(has);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking forced-delete profiles for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> CheckResidentIsCurrentAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: A "current" resident means the record is not linked to a newer staging match.
            //       Implement once the staging/linking data model is fully defined.
            var exists = await readDb.Residents.AnyAsync(r => r.Id == residentId, cancellationToken);
            return Results.Ok(exists);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if resident {ResidentId} is current record", residentId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Med Profiles ───────────────────────────────────────────────────────────

    public async Task<IResult> GetMedProfilesAsync(Guid residentId, bool isActive, int page, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var query = readDb.MedProfiles
                .Where(p => p.ResidentId == residentId && p.IsActive == isActive);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(new PagedResponse<MedProfileDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving med profiles for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred while retrieving medication profiles.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> CheckMedProfileCanBeRemovedAsync(Guid profileId, CancellationToken cancellationToken)
    {
        try
        {
            // A profile can be removed when it is not locked and not the sole active profile.
            // TODO: Extend this rule once the full locking and active-profile constraints are known.
            var profile = await readDb.MedProfiles.FindAsync([profileId], cancellationToken);
            if (profile is null) return Results.NotFound();

            var canRemove = string.IsNullOrEmpty(profile.LockStatus) || profile.LockStatus == "Unlocked";
            return Results.Ok(canRemove);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if med profile {ProfileId} can be removed", profileId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> RemoveMedProfileAsync(Guid profileId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await db.MedProfiles.FindAsync([profileId], cancellationToken);
            if (profile is null) return Results.NotFound();

            // TODO: Validate removal rules (e.g. cannot remove the only active profile).
            db.MedProfiles.Remove(profile);
            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync(CacheTag, cancellationToken);

            logger.LogInformation("MedProfile {ProfileId} removed for resident {ResidentId}", profileId, profile.ResidentId);
            return Results.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing med profile {ProfileId}", profileId);
            return Results.Problem("An error occurred while removing the medication profile.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> RemovePendingChartProfileAsync(RemovePendingChartProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await db.MedProfiles.FindAsync([request.MedProfileId], cancellationToken);
            if (profile is null) return Results.NotFound();

            // Optimistic concurrency: compare LatestChangedDate
            if (request.LatestChangedDate.HasValue &&
                profile.LastChangedDate.HasValue &&
                profile.LastChangedDate.Value > request.LatestChangedDate.Value)
            {
                return Results.Ok(new LockProfileDto
                {
                    LockStatus = "Locked",
                    LockByName = profile.LockedByName,
                    LockById = profile.LockedById,
                    LockedAt = profile.LockedAt,
                    IsSameMedProfileId = true
                });
            }

            db.MedProfiles.Remove(profile);
            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync(CacheTag, cancellationToken);

            return Results.Ok(new LockProfileDto { LockStatus = "Unlocked" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing pending chart profile {ProfileId}", request.MedProfileId);
            return Results.Problem("An error occurred while removing the pending chart profile.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> IsProfileEditableAsync(Guid profileId, string status, string lastUpdated, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await readDb.MedProfiles
                .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);

            if (profile is null) return Results.NotFound();

            // TODO: Replicate the legacy IsProfileEditable string-status logic once
            //       the full set of status values and their editability rules are known.
            var isEditable = profile.LockStatus != "Locked" && profile.Status == status;
            return Results.Ok(isEditable ? "Editable" : "NotEditable");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking editability of profile {ProfileId}", profileId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> CheckMedProfileLockAsync(Guid profileId, string lastUpdated, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await readDb.MedProfiles
                .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);

            if (profile is null) return Results.NotFound();

            return Results.Ok(new LockProfileDto
            {
                LockStatus = profile.LockStatus,
                LockByName = profile.LockedByName,
                LockById = profile.LockedById,
                LockedAt = profile.LockedAt,
                IsSameMedProfileId = null
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking lock for profile {ProfileId}", profileId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetInvalidPreferredBrandPbsAsync(Guid profileId, CancellationToken cancellationToken)
    {
        // TODO: Query preferred-brand PBS items for the profile and return any that are invalid.
        //       Requires the preferred-brand PBS data model to be defined.
        logger.LogWarning("GetInvalidPreferredBrandPBS called for profile {ProfileId} — not yet implemented", profileId);
        return await Task.FromResult(Results.Ok(Array.Empty<object>()));
    }

    public async Task<IResult> CompleteMedChangeVerificationAsync(Guid medProfileId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await db.MedProfiles.FindAsync([medProfileId], cancellationToken);
            if (profile is null) return Results.NotFound();

            // TODO: Mark the profile as verified / clear the "updated by doctor" pending flag.
            //       The exact status transition depends on the legacy MedProfile state machine.
            profile.LastUpdatedDate = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync(CacheTag, cancellationToken);

            return Results.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error completing medicine change verification for profile {ProfileId}", medProfileId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Pack & Scheduling ──────────────────────────────────────────────────────

    public async Task<IResult> GetPackLayoutAsync(Guid residentId, Guid? medProfileId, CancellationToken cancellationToken)
    {
        // TODO: Query PackLayout records. Pack layout data likely lives in the pharmacy/pack domain.
        //       Coordinate ownership before implementing.
        logger.LogWarning("GetPackLayout called for resident {ResidentId} — not yet implemented", residentId);
        return await Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public async Task<IResult> SaveGroupPackAsync(Dictionary<string, string> groupPacks, CancellationToken cancellationToken)
    {
        // TODO: Persist group pack assignments. Data model TBD.
        logger.LogWarning("SaveGroupPack called with {Count} entries — not yet implemented", groupPacks.Count);
        return await Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public async Task<IResult> CheckCleanProfileAsync(Guid residentId, DateTime dateTime, CancellationToken cancellationToken)
    {
        // TODO: Validate whether the profile can be cleaned at the given date.
        //       Requires schedule/dose-round rules from the facility domain.
        logger.LogWarning("CheckCleanProfile called for resident {ResidentId} — not yet implemented", residentId);
        return await Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public async Task<IResult> CleanProfileAsync(Guid residentId, CleanProfileRequest request, CancellationToken cancellationToken)
    {
        // TODO: Execute the clean-profile operation (remove future scheduled doses, optionally
        //       create a dose signing record). Multi-step; requires schedule domain knowledge.
        logger.LogWarning("CleanProfile called for resident {ResidentId} — not yet implemented", residentId);
        return await Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public async Task<IResult> GenerateDoseSigningAsync(Guid residentId, GenerateDoseSigningRequest request, CancellationToken cancellationToken)
    {
        // TODO: Generate the dose signing record for the given date/time.
        //       Requires DoseRound and FacilityDoseConfig from the facility domain.
        logger.LogWarning("GenerateDoseSigning called for resident {ResidentId} — not yet implemented", residentId);
        return await Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public async Task<IResult> UpdateRegSchedulingAsync(Guid userId, UpdateRegSchedulingRequest request, CancellationToken cancellationToken)
    {
        // TODO: Persist the regular scheduling changes. userId must come from JWT — never from
        //       request body (§3 Critical Rule). This operation is complex and schedule-domain-heavy.
        logger.LogWarning("UpdateRegScheduling called by user {UserId} — not yet implemented", userId);
        return await Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    // ── Prescribers ────────────────────────────────────────────────────────────

    public async Task<IResult> GetPrescribersDropdownAsync(Guid residentId, int type, CancellationToken cancellationToken)
    {
        // TODO: Query the resident→prescriber association table.
        //       Currently returns an empty list; implement once the prescriber-resident
        //       join table is mapped to an entity.
        logger.LogWarning("GetPrescribersDropdown called for resident {ResidentId} — not yet implemented", residentId);

        var items = new List<PrescriberDropdownDto>();

        // Apply prefix items per the legacy type rules (§4.18)
        if (type == 1)
            items.Insert(0, new PrescriberDropdownDto { Text = "Select", Value = string.Empty });
        else if (type != 2)
            items.Insert(0, new PrescriberDropdownDto { Text = "All", Value = "0" });

        return await Task.FromResult(Results.Ok(items));
    }

    // ── VMC flags ──────────────────────────────────────────────────────────────

    public async Task<IResult> UpdateVmcRequireTransferAsync(Guid residentId, bool setBackToNull, CancellationToken cancellationToken)
    {
        try
        {
            var resident = await db.Residents.FindAsync([residentId], cancellationToken);
            if (resident is null) return Results.NotFound();

            // setBackToNull=true clears the flag; setBackToNull=false sets it (BMED-10406)
            resident.VMCRequireTransfer = setBackToNull ? null : true;
            resident.LastUpdatedDate = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync(CacheTag, cancellationToken);

            return Results.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating VMCRequireTransfer for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> CheckVmcRequireTransferAndActiveProfileAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            var resident = await readDb.Residents
                .FirstOrDefaultAsync(r => r.Id == residentId, cancellationToken);

            if (resident is null) return Results.NotFound();

            var hasActiveProfile = await readDb.MedProfiles
                .AnyAsync(p => p.ResidentId == residentId && p.IsActive, cancellationToken);

            return Results.Ok(resident.VMCRequireTransfer == true && hasActiveProfile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking VMC require transfer for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Staging & Linking ──────────────────────────────────────────────────────

    public async Task<IResult> GetResidentStagingMatchedAsync(Guid residentId, CancellationToken cancellationToken)
    {
        // TODO: Query the ResidentStaging table. Entity not yet mapped — add to DbContext
        //       and create a ResidentStaging entity once the staging schema is confirmed.
        logger.LogWarning("GetResidentStagingMatched called for {ResidentId} — not yet implemented", residentId);
        return await Task.FromResult(Results.Ok(Array.Empty<object>()));
    }

    public async Task<IResult> GetMatchedResidentsAsync(Guid residentId, CancellationToken cancellationToken)
    {
        // TODO: Query matched residents from the BPack linking data.
        logger.LogWarning("GetMatchedResidents called for {ResidentId} — not yet implemented", residentId);
        return await Task.FromResult(Results.Ok(Array.Empty<object>()));
    }

    public async Task<IResult> LinkResidentAsync(Guid residentId, LinkResidentRequest request, CancellationToken cancellationToken)
    {
        // TODO: Persist the resident link (residentId ↔ matchedResidentId).
        //       May require a ResidentLink join table. ConfirmText is only needed for
        //       scenarios where a confirmation dialog is shown to the user.
        logger.LogWarning("LinkResident called for {ResidentId} → {MatchedId} — not yet implemented",
            residentId, request.MatchedResidentId);
        return await Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    // ── Facility offline mode ──────────────────────────────────────────────────

    public async Task<IResult> IsFacilityOfflineModeAsync(Guid residentId, CancellationToken cancellationToken)
    {
        try
        {
            var facilityId = await readDb.Residents
                .Where(r => r.Id == residentId)
                .Select(r => (Guid?)r.FacilityId)
                .FirstOrDefaultAsync(cancellationToken);

            if (facilityId is null) return Results.NotFound();

            var isOffline = await facilityClient.IsFacilityOfflineModeAsync(facilityId.Value, cancellationToken);
            return Results.Ok(isOffline);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking offline mode for resident {ResidentId}", residentId);
            return Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
