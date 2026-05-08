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
}
