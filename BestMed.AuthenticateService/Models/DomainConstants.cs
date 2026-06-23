namespace BestMed.AuthenticateService.Models;

public static class UserTypes
{
    public const string Facility = "Facility";
    public const string Pharmacy = "Pharmacy";
    public const string Warehouse = "Warehouse";
    public const string AHService = "AHService";
}

public static class UserRoleCodes
{
    public const string FDoctor = "FDoctor";
    public const string FDietician = "FDietician";
    public const string FHealthDepartmentInspector = "FHealthDepartmentInspector";
    public const string FWitness = "FWitness";
    public const string FAgencyRegisteredNurse = "FAgencyRegisteredNurse";
    public const string FAgencyAIN = "FAgencyAIN";
    public const string FAgencyEnrolledNurse = "FAgencyEnrolledNurse";
    public const string FAgencyNDISWorker = "FAgencyNDISWorker";

    public static readonly HashSet<string> AgencyRoles =
    [
        FAgencyRegisteredNurse,
        FAgencyAIN,
        FAgencyEnrolledNurse,
        FAgencyNDISWorker
    ];

    public static readonly HashSet<string> AgencyAndWitnessRoles =
    [
        FAgencyRegisteredNurse,
        FAgencyAIN,
        FAgencyEnrolledNurse,
        FAgencyNDISWorker,
        FWitness
    ];

    public static readonly HashSet<string> PasswordExpiryExcludeRoles =
    [
        "FIntegration",
        "PIntegration",
        "PNonBESTMEDPharmacist",
        FAgencyRegisteredNurse,
        FAgencyAIN,
        FAgencyEnrolledNurse,
        FAgencyNDISWorker
    ];
}

public static class AreaNames
{
    public const string BESTdose = "BESTdose";
    public const string BESTdoctor = "BESTdoctor";
    public const string BESTtrack = "BESTtrack";
    public const string BESTpackV2 = "BESTpack (V2)";
    public const string BESTservice = "BESTservice";
}

public static class FacilityTypes
{
    public const string BESTtrack = "BESTtrack";
}

public static class UserStatus
{
    public const string Active = "Active";
}
