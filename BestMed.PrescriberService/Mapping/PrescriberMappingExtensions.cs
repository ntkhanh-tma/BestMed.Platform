using BestMed.Common.Constants;
using BestMed.PrescriberService.DTOs;
using BestMed.PrescriberService.Entities;

namespace BestMed.PrescriberService.Mapping;

public static class PrescriberMappingExtensions
{
    public static PrescriberDto ToDto(this Prescriber entity) => new()
    {
        Id = entity.Id,
        PrescriberName = entity.PrescriberName,
        PrescriberCode = entity.PrescriberCode,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        PreferredName = entity.PreferredName,
        Email = entity.Email,
        Phone = entity.Phone,
        MobileNumber = entity.MobileNumber,
        Fax = entity.Fax,
        OutOfHours = entity.OutOfHours,
        Address1 = entity.Address1,
        Address2 = entity.Address2,
        Suburb = entity.Suburb,
        State = entity.State,
        Postcode = entity.Postcode,
        Country = entity.Country,
        AHPRANumber = entity.AHPRANumber,
        HPIINumber = entity.HPIINumber,
        HPIIStatus = entity.HPIIStatus,
        LicenseNumber = entity.LicenseNumber,
        Qualification = entity.Qualification,
        PinAcknowledge = entity.PinAcknowledge,
        EnableMimsDrugInteractionChecking = entity.EnableMimsDrugInteractionChecking,
        IseRxUserAccessAgreementAccepted = entity.IseRxUserAccessAgreementAccepted,
        ERxUserAccessAgreementAcceptedDate = entity.ERxUserAccessAgreementAcceptedDate,
        ERxUserAccessAgreementVersion = entity.ERxUserAccessAgreementVersion,
        ERxEntityId = entity.ERxEntityId
    };

    public static void ApplyTo(this UpdatePrescriberRequest request, Prescriber prescriber)
    {
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
    }

    public static IQueryable<Prescriber> ApplyFilters(this IQueryable<Prescriber> queryable, PrescriberQueryParameters query)
    {
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

        return queryable;
    }

    public static IQueryable<Prescriber> ApplySorting(this IQueryable<Prescriber> queryable, PrescriberQueryParameters query)
    {
        var asc = SortDirection.IsAscending(query.SortDirection);
        return query.SortBy?.ToLowerInvariant() switch
        {
            "prescribercode" => asc ? queryable.OrderBy(p => p.PrescriberCode) : queryable.OrderByDescending(p => p.PrescriberCode),
            "firstname" => asc ? queryable.OrderBy(p => p.FirstName) : queryable.OrderByDescending(p => p.FirstName),
            "lastname" => asc ? queryable.OrderBy(p => p.LastName) : queryable.OrderByDescending(p => p.LastName),
            _ => asc ? queryable.OrderBy(p => p.PrescriberName) : queryable.OrderByDescending(p => p.PrescriberName)
        };
    }
}
