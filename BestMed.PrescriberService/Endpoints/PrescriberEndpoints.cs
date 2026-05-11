using BestMed.Common.Constants;
using BestMed.Common.Models;
using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.PrescriberService.Data;
using BestMed.PrescriberService.DTOs;
using BestMed.PrescriberService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PrescriberService.Endpoints;

public static class PrescriberEndpoints
{
    public static IEndpointRouteBuilder MapPrescriberEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/prescribers")
            .WithTags("Prescribers")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetPrescriberById")
            .WithDescription("Get a single prescriber by ID")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", QueryAsync)
            .WithName("QueryPrescribers")
            .WithDescription("Search and filter prescribers with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdatePrescriber")
            .WithDescription("Update a single prescriber")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ReadOnlyPrescriberDbContext db,
        CancellationToken cancellationToken)
    {
        var prescriber = await db.Prescribers
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return prescriber is null
            ? Results.NotFound()
            : Results.Ok(prescriber.ToDto());
    }

    private static async Task<IResult> QueryAsync(
        [AsParameters] PrescriberQueryParameters query,
        ReadOnlyPrescriberDbContext db,
        CancellationToken cancellationToken)
    {
        var queryable = db.Prescribers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.PrescriberName))
            queryable = queryable.Where(p => p.PrescriberName.Contains(query.PrescriberName));

        if (!string.IsNullOrWhiteSpace(query.PrescriberCode))
            queryable = queryable.Where(p => p.PrescriberCode == query.PrescriberCode);

        if (!string.IsNullOrWhiteSpace(query.FirstName))
            queryable = queryable.Where(p => p.FirstName != null && p.FirstName.Contains(query.FirstName));

        if (!string.IsNullOrWhiteSpace(query.LastName))
            queryable = queryable.Where(p => p.LastName != null && p.LastName.Contains(query.LastName));

        if (!string.IsNullOrWhiteSpace(query.Email))
            queryable = queryable.Where(p => p.Email != null && p.Email.Contains(query.Email));

        if (!string.IsNullOrWhiteSpace(query.AHPRANumber))
            queryable = queryable.Where(p => p.AHPRANumber == query.AHPRANumber);

        var asc = SortDirection.IsAscending(query.SortDirection);
        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "prescribercode" => asc
                ? queryable.OrderBy(p => p.PrescriberCode)
                : queryable.OrderByDescending(p => p.PrescriberCode),
            "firstname" => asc
                ? queryable.OrderBy(p => p.FirstName)
                : queryable.OrderByDescending(p => p.FirstName),
            "lastname" => asc
                ? queryable.OrderBy(p => p.LastName)
                : queryable.OrderByDescending(p => p.LastName),
            _ => asc
                ? queryable.OrderBy(p => p.PrescriberName)
                : queryable.OrderByDescending(p => p.PrescriberName)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => p.ToDto())
            .ToListAsync(cancellationToken);

        return Results.Ok(new PagedResponse<PrescriberDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdatePrescriberRequest request,
        PrescriberDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
    {
        var prescriber = await db.Prescribers.FindAsync([id], cancellationToken);
        if (prescriber is null) return Results.NotFound();

        if (request.PrescriberName is not null) prescriber.PrescriberName = request.PrescriberName;
        if (request.PrescriberCode is not null) prescriber.PrescriberCode = request.PrescriberCode;
        if (request.FirstName is not null) prescriber.FirstName = request.FirstName;
        if (request.LastName is not null) prescriber.LastName = request.LastName;
        if (request.PreferredName is not null) prescriber.PreferredName = request.PreferredName;
        if (request.Email is not null) prescriber.Email = request.Email;
        if (request.Phone is not null) prescriber.Phone = request.Phone;
        if (request.MobileNumber is not null) prescriber.MobileNumber = request.MobileNumber;
        if (request.Fax is not null) prescriber.Fax = request.Fax;
        if (request.OutOfHours is not null) prescriber.OutOfHours = request.OutOfHours;
        if (request.Address1 is not null) prescriber.Address1 = request.Address1;
        if (request.Address2 is not null) prescriber.Address2 = request.Address2;
        if (request.Suburb is not null) prescriber.Suburb = request.Suburb;
        if (request.State is not null) prescriber.State = request.State;
        if (request.Postcode is not null) prescriber.Postcode = request.Postcode;
        if (request.Country is not null) prescriber.Country = request.Country;
        if (request.AHPRANumber is not null) prescriber.AHPRANumber = request.AHPRANumber;
        if (request.HPIINumber is not null) prescriber.HPIINumber = request.HPIINumber;
        if (request.HPIIStatus is not null) prescriber.HPIIStatus = request.HPIIStatus;
        if (request.LicenseNumber is not null) prescriber.LicenseNumber = request.LicenseNumber;
        if (request.Qualification is not null) prescriber.Qualification = request.Qualification;
        if (request.EnableMimsDrugInteractionChecking.HasValue) prescriber.EnableMimsDrugInteractionChecking = request.EnableMimsDrugInteractionChecking.Value;
        if (request.DefaultMimsSeverityLevel is not null) prescriber.DefaultMimsSeverityLevel = request.DefaultMimsSeverityLevel;
        if (request.DefaultMimsDocumentationLevel is not null) prescriber.DefaultMimsDocumentationLevel = request.DefaultMimsDocumentationLevel;
        if (request.IseRxUserAccessAgreementAccepted.HasValue) prescriber.IseRxUserAccessAgreementAccepted = request.IseRxUserAccessAgreementAccepted.Value;
        if (request.ERxUserAccessAgreementVersion is not null) prescriber.ERxUserAccessAgreementVersion = request.ERxUserAccessAgreementVersion;
        if (request.ERxEntityId is not null) prescriber.ERxEntityId = request.ERxEntityId;

        await db.SaveChangesAsync(cancellationToken);
        await cache.EvictByTagAsync("prescribers", cancellationToken);

        // Notify other services (e.g. UserService) that prescriber data has changed.
        // Pattern: Service Bus (async) — fire-and-forget, consumers invalidate their caches.
        await eventPublisher.PublishAsync(new PrescriberUpdatedEvent
        {
            PrescriberId = prescriber.Id,
            PrescriberName = prescriber.PrescriberName,
            PrescriberCode = prescriber.PrescriberCode
        }, cancellationToken);

        return Results.Ok(prescriber.ToDto());
    }
}
