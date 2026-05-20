# PHARMACY DOMAIN — BESTmedBAT / dbo
> For C# EF Core code generation. Schema: dbo. All PKs are Guid (newid()). ClusteredKey (long, NOT PK) on all tables. Audit cols: CreatedDate(datetime?), CreatedBy(Guid?), LastUpdatedDate(datetime?), LastUpdatedBy(Guid?) unless noted.

## CONVENTIONS
- PK: Guid, ValueGeneratedOnAdd()
- ClusteredKey: long, not PK, map as property
- Audit base class: AuditableEntity { CreatedDate?, CreatedBy?, LastUpdatedDate?, LastUpdatedBy? }
- nvarchar(max)/varchar(max) → string (no MaxLength)
- bit NOT NULL → bool; bit NULL → bool?
- All FKs in dbo schema unless noted

---

## Pharmacy [ROOT AGGREGATE]
FK→ Warehouse.Id, StateTimeZone.Id

| Column | C# Type | MaxLen | Default |
|--------|---------|--------|---------|
| Id | Guid | | newid() |
| Name | string | 100 | |
| WarehouseId | Guid? | | |
| StateTimeZoneId | Guid? | | |
| Active | bool | | true |
| PharmacyType | int? | | 0 |
| Address1 | string? | 100 | |
| Address2 | string? | 100 | |
| Suburb | string? | 50 | |
| State | string? | 30 | |
| PostCode | string? | 4 | |
| Country | string? | 30 | |
| ShippingAddress1 | string? | 100 | |
| ShippingAddress2 | string? | 100 | |
| ShippingState | string? | 30 | |
| ShippingSuburb | string? | 50 | |
| ShippingPostCode | string? | 4 | |
| IsUseBillingAddress | bool | | true |
| BillingName | string? | 200 | |
| BillingContactId | Guid? | | |
| ContactName | string? | 50 | |
| Phone | string? | 20 | |
| Fax | string? | 20 | |
| Email | string? | 255 | |
| OutOfHours | string? | 20 | |
| ABN | string? | 11 | |
| PharmacyApprovalNumber | string? | 20 | |
| HPIONumber | string? | 20 | |
| HPIOStatus | string? | max | |
| HasPackingFacility | bool? | | |
| S8DrugPackingAllowed | bool? | | |
| IsMultiSite | bool | | false |
| IsInDispenseMigration | bool | | false |
| LastDispenseMigrationDate | DateTime? | | |
| AutoSyncDispenseResident | bool | | true |
| DispenseSystemType | int? | | |
| FinancialType | int? | | |
| MinValue | decimal? | | |
| MaxValue | decimal? | | |
| Discount | decimal? | | |
| DaysInvoiceTerms | int? | | |
| EnablePasswordAging | bool? | | |
| PasswordAging | int? | | |
| WorkingDays | string? | 14 | |
| GeoLocations | string? | max | |
| GeoRadius | double? | | |
| IPAddress | string? | max | |
| IPDescription | string? | 500 | |
| XMLUserName | string? | 100 | '' |
| XMLUserPassword | string? | 150 | '' |
| DropboxToken | string? | 100 | |
| FredNXTAccessToken | string? | 200 | |
| LastFredNXTResidentSyncDate | DateTime? | | |
| LastFredNXTScriptSyncDate | DateTime? | | |
| LastFredNXTPrescriberSyncDate | DateTime? | | |
| BCPChartGenerationEnabled | bool | | false |
| BCPSigningSheetGenerationEnabled | bool | | false |
| EnablePackScheduleAPI | bool? | | |
| PackScheduleAPIEnabledDate | DateTime? | | |
| EnableDashboard | bool | | true |
| EnableDistributedPacking | bool? | | |
| IsBestpackCore | bool? | | |
| HomeCareExclude | bool | | false |
| AllowPreferredBrandSubstitution | bool? | | |
| AllowAllClaims | bool? | | |
| IsAutoFacilityReport | bool? | | |
| AutoFacilityReportGenerateDate | int? | | |
| IsAutoComplianceReport | bool? | | |
| IsAutoBulkMedChart | bool? | | |
| HasCPIClause | bool | | false |
| EnableAddMoveResidentViaBpack | bool? | | |
| ProgrammeJoinedDate | DateOnly? | | |
| Tier | string? | 20 | |
| MedicationTrackingReturnedReason | string? | max | |
| S8DestructionEmail | string? | 150 | |
| UpdateNrmcCompliantForAudit | bool? | | |
| ArchiveFredUsedRepeat | bool? | | |
| PriceModel | Guid? | | |
| CheckingMachineType | Guid? | | |
| TermsAndConditions | string? | max | |
| TermsAndConditionsType | string? | 15 | |
| ResidentsFacilityCode | string? | 10 | |
| ClaimName | string? | 25 | |
| ClaimQualification | string? | 25 | |
| SachetRobotTypeId | string? | max | |
| BlisterRobotTypeId | string? | max | |
| YuyamaModelId | string? | max | |
| SachetPackingMachineInstalledDate | DateOnly? | | |
| SachetPackingMachineAssetNumber | string? | 50 | |
| SachetPackingMachineInstalledDateSecond | DateOnly? | | |
| SachetPackingMachineAssetNumberSecond | string? | 50 | |
| BlisterPackingMachineInstalledDate | DateOnly? | | |
| BlisterPackingMachineAssetNumber | string? | 50 | |
| BlisterPackingMachineInstalledDateSecond | DateOnly? | | |
| BlisterPackingMachineAssetNumberSecond | string? | 50 | |
| CheckingMachineInstalledDate | DateOnly? | | |
| CheckingMachineAssetNumber | string? | 50 | |
| CheckingMachineInstalledDateSecond | DateOnly? | | |
| CheckingMachineAssetNumberSecond | string? | 50 | |
| CheckingSoftwareMaintenanceFee | decimal? | | |
| ResidentFacilityFeeLessThanOrEqualTo1000 | decimal? | | |
| ResidentFacilityFeeGreaterThan1000 | decimal? | | |
| BESTpackCommunityFee | decimal? | | |
| BESTdoctorResidentFee | decimal? | | |
| BESTpackMonthlySupportFee | decimal? | | |
| BESTpackDoctorIntegrationFee | decimal? | | |
| OtherInformation | string? | 500 | |
| ClusteredKey | long | | |

NAV PROPS: ICollection<UserPharmacy>, ICollection<BCPSetting>, ICollection<BESTMEDSupplyPharmacy>, ICollection<PackRequest>, ICollection<BatchReferenceFileBuilder>, ICollection<PharmacyInvoiceDocument>, ICollection<QuarterlyPharmacyGroup>, ICollection<Facility>

---

## UserPharmacy : AuditableEntity
M2M: User ↔ Pharmacy

| Column | C# Type |
|--------|---------|
| Id | Guid |
| UserId | Guid | FK→User
| PharmacyId | Guid | FK→Pharmacy
| ClusteredKey | long |

---

## BCPSetting
1:1 with Pharmacy (one per pharmacy)

| Column | C# Type | MaxLen |
|--------|---------|--------|
| Id | Guid | |
| PharmacyId | Guid | FK→Pharmacy |
| DropboxToken | string? | 100 |
| OneDriveToken | string? | max |
| OneDriveTokenExpired | DateTime? | |
| OneDriveExpired | DateTime? | |
| ClusteredKey | long | |

NO audit cols.

---

## BESTMEDSupplyPharmacy : AuditableEntity
Facility–Pharmacy supply config (many per pharmacy)
FK→ Pharmacy.Id, Facility.Id

| Column | C# Type | MaxLen | Default |
|--------|---------|--------|---------|
| Id | Guid | | |
| PharmacyId | Guid | | FK |
| FacilityId | Guid | | FK |
| FacilityCode | string | 20 | |
| IsActive | bool | | |
| BulkPackDayOffset | int? | | |
| NonPackDeliveryDay | string? | 10 | |
| S8DrugPackingAllowed | bool? | | |
| NRMCCompliantDefault | bool? | | |
| VariablePackType | string? | 20 | |
| PrnPackType | string? | 20 | |
| ShortCoursePackType | string? | 20 | |
| CytotoxicPackType | string? | 20 | |
| AntibioticsPackType | string? | 20 | |
| AntimicrobialPackTypeShortCourse | string? | 20 | |
| S4DPackType | string | 10 | 'NP' |
| PackForm | string? | 20 | |
| DualPackingEnabled | bool? | | |
| DefaultPackHeaderType | string? | 20 | |
| DefaultPackingLocation | string? | 20 | |
| S8ToBePackedSeparately | bool? | | |
| CytotoxicToBePackedSeparately | bool? | | |
| CytostaticToBePackedSeparately | bool? | | |
| VariableToBePackedSeparately | bool? | | |
| S4DToBePackedSeparately | bool? | | |
| DoNotCrushToBePackedSeparately | bool? | | |
| PackDNCandRegularInSamePackRoll | bool? | | |
| ShowMedicationOnFrontOfThePack | bool? | | |
| IsBlisterStartFromBottom | bool? | | |
| PackedPRNExpiryPeriodInMonths | int? | | |
| RegularExpiry | int? | | |
| S8Expiry | int? | | |
| SachetPrintTime | int? | | |
| CommunitySachetLayout | bool? | | |
| UtiliseAdditionalBulkRolls | bool? | | |
| RobotTypeId | Guid? | | |
| DistributedPackingRobotTypeId | Guid? | | |
| YuyamaModelId | Guid? | | |
| DistributedPackingYuyamaModelId | Guid? | | |
| AllowAliasing | bool? | | |
| FacilityLiveDate | DateTime? | | |
| ForceDownload | bool? | | |
| ForceMedChart | bool? | | |
| MedChangeCutoffTime | string? | 10 | |
| OnlineOrderingCutoffTime | string? | 10 | |
| PrintClaimsSeparately | bool? | | |
| S8ReportDay | string? | 10 | |
| PharmacyEyeDropDeliveryDay | string? | 10 | |
| EmergencyStockPatientNumber | string? | 10 | |
| EnableMedicationTracking | bool? | | |
| MedicationTrackingDispatchLocation | string? | 250 | |
| ClusteredKey | long | | |

---

## SupplyPharmacy [SEPARATE AGGREGATE]
Non-BESTmed supply pharmacies. Audit cols NOT NULL here.

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| Name | string | 100 | No |
| FacilityId | Guid | | No |
| IsActive | bool | | No (true) |
| Address1 | string? | 100 | Yes |
| Address2 | string? | 100 | Yes |
| Suburb | string? | 50 | Yes |
| State | string? | 50 | Yes |
| PostCode | string? | 4 | Yes |
| Country | string? | 30 | Yes |
| Phone | string? | 20 | Yes |
| Fax | string? | 20 | Yes |
| Email | string? | 255 | Yes |
| OutOfHours | string? | 20 | Yes |
| IPAddress | string? | max | Yes |
| CreatedDate | DateTime | | No |
| CreatedBy | Guid | | No |
| LastUpdatedDate | DateTime | | No |
| LastUpdatedBy | Guid | | No |
| ClusteredKey | long | | No |

NAV PROPS: ICollection<NonBhsUserPharmacy>, ICollection<SupplyPharmacySection>

---

## SupplyPharmacySection : AuditableEntity
FK→ SupplyPharmacy.Id (as PharmacyId), Section.Id

| Column | C# Type | MaxLen |
|--------|---------|--------|
| Id | Guid | |
| FacilityId | Guid | |
| PharmacyId | Guid | FK→SupplyPharmacy |
| SectionId | Guid | FK→Section |
| SectionCode | string | 20 |
| ClusteredKey | long | |

---

## NonBhsUserPharmacy : AuditableEntity
M2M: User ↔ SupplyPharmacy

| Column | C# Type |
|--------|---------|
| Id | Guid |
| UserId | Guid | FK→User
| PharmacyId | Guid | FK→SupplyPharmacy
| ClusteredKey | long |

---

## PackRequest
FK→ Pharmacy.Id, Facility.Id, User.Id (PackRequestBy)

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| PharmacyId | Guid | | No |
| FacilityId | Guid | | No |
| SectionId | Guid? | | Yes |
| RequestNumber | string | 20 | No |
| PackRequestDate | DateTime | | No |
| PackRequestBy | Guid | | No |
| PackFrom | DateTime? | | Yes |
| PackTo | DateTime? | | Yes |
| PackRequestType | int | | No |
| Status | int | | No |
| WarehouseStatus | int? | | Yes |
| Generatedby | string | 1 | No |
| PackDocumentId | Guid? | | Yes |
| BillDocumentId | Guid? | | Yes |
| ProcessId | Guid? | | Yes |
| IsVerified | bool? | | Yes |
| BillStatus | bool? | | Yes |
| RobotTypeId | Guid? | | Yes |
| Packer | string? | 255 | Yes |
| VMCReverseDate | DateTime? | | Yes |
| VMCReverseBy | Guid? | | Yes |
| PackRequestLocationId | int? | | Yes |

NO ClusteredKey. NO audit cols.
NAV PROPS: ICollection<PackResidentRoll>

---

## PackResidentRoll
FK→ PackRequest.Id, Resident.Id

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| PackRequestId | Guid | | No |
| ResidentId | Guid | | No |
| PackType | int | | No |
| ActiveMedProfileId | Guid | | No |
| PrevMedProfileId | Guid? | | Yes |
| ResidentPackFrom | DateTime? | | Yes |
| ResidentPackTo | DateTime? | | Yes |
| BillPackQtyDays | int? | | Yes |
| IsS8 | bool? | | Yes |
| IsAccepted | bool? | | Yes |
| RejectReason | string? | 255 | Yes |
| DoNotCharge | bool? | | Yes |
| VerifiedBy | Guid? | | Yes |
| VerifiedDate | DateTime? | | Yes |
| RollNumber | string? | 4 | Yes |
| IsIssued | bool | | false |
| Disposed | bool? | | Yes |
| ReturnedtoPharmacy | bool? | | Yes |
| Discharged | bool? | | Yes |
| MedicationTrackingActionId | Guid? | | Yes |
| TrackedBy | Guid? | | Yes |
| TrackedDate | DateTime? | | Yes |
| MedicationTrackingLastLocation | string? | 200 | Yes |
| MedicationTrackingCurrentLocation | string? | 200 | Yes |
| ClusteredKey | long | | No |

NAV PROPS: ICollection<PackResidentMed>

---

## PackResidentMed
FK→ PackResidentRoll.Id, Medicine.Id

| Column | C# Type | Nullable |
|--------|---------|---------|
| Id | Guid | No |
| PackResidentRollId | Guid | No |
| MedicineId | Guid | No |
| DrugId | Guid | No |
| DrugType | string(20) | No |
| PRNDoses | int? | Yes |
| BillPackQtyWhole | decimal? | Yes |
| BillPackQtyFractional | int? | Yes |
| DoseQty | decimal? | Yes |
| ClusteredKey | long | No |

---

## BatchReferenceFileBuilder
FK→ Pharmacy.Id, Document.Id
NO ClusteredKey. NO audit cols (has CreatedDate/CreatedBy only).

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| PharmacyId | Guid | | No |
| DocumentId | Guid | | No |
| PackRequestNumber | string | 20 | No |
| MedicationCount | int? | | Yes |
| CreatedDate | DateTime | | No |
| CreatedBy | Guid | | No |

---

## PharmacyInvoiceDocument
FK→ Pharmacy.Id, Document.Id

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| PharmacyId | Guid | | No |
| DocumentId | Guid | | No |
| InvoiceNumber | string | 20 | No |
| InvoiceStatus | string | 20 | No |
| CreatedDate | DateTime? | | Yes |
| ClusteredKey | long | | No |

---

## QuarterlyPharmacyGroup
FK→ Pharmacy.Id, QuarterlyGroup.Id
NO audit cols.

| Column | C# Type |
|--------|---------|
| Id | Guid |
| PharmacyId | Guid |
| QuarterlyGroupId | Guid |
| ClusteredKey | long |

---

## REPOSITORIES NEEDED
| Interface | Aggregate Root | Notes |
|-----------|---------------|-------|
| IPharmacyRepository | Pharmacy | Include BCPSetting, UserPharmacy, BESTMEDSupplyPharmacy as nav props |
| ISupplyPharmacyRepository | SupplyPharmacy | Include NonBhsUserPharmacy, SupplyPharmacySection |
| IPackRequestRepository | PackRequest | Include PackResidentRoll → PackResidentMed hierarchy |
| IPharmacyInvoiceRepository | PharmacyInvoiceDocument | Standalone |