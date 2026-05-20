# FACILITY DOMAIN — BESTmedBAT / dbo
> For C# EF Core code generation. Schema: dbo. All PKs are Guid (newid()). ClusteredKey (long, NOT PK) on most tables. Audit cols: CreatedDate(datetime?), CreatedBy(Guid?), LastUpdatedDate(datetime?), LastUpdatedBy(Guid?) unless noted.

## CONVENTIONS
- PK: Guid, ValueGeneratedOnAdd()
- ClusteredKey: long, not PK, map as property
- Audit base class: AuditableEntity { CreatedDate?, CreatedBy?, LastUpdatedDate?, LastUpdatedBy? }
- nvarchar(max)/varchar(max) → string (no MaxLength)
- bit NOT NULL → bool; bit NULL → bool?
- int NOT NULL Active col on Facility (not bool)

---

## Facility [ROOT AGGREGATE]
FK→ Pharmacy.Id, StateTimeZone.Id

| Column | C# Type | MaxLen | Nullable | Default |
|--------|---------|--------|---------|---------|
| Id | Guid | | No | newid() |
| Name | string | 100 | No | |
| PharmacyId | Guid | | No | FK→Pharmacy |
| StateTimeZoneId | Guid? | | Yes | FK→StateTimeZone |
| Active | int | | No | 1 |
| FacilityType | string? | 20 | Yes | |
| ContactName | string? | 100 | Yes | |
| FredCode | string? | 20 | Yes | |
| RacId | string? | 10 | Yes | |
| Address1 | string? | 200 | Yes | |
| Address2 | string? | 100 | Yes | |
| Suburb | string? | 20 | Yes | |
| State | string? | 20 | Yes | |
| PostCode | string? | 10 | Yes | |
| Country | string? | 20 | Yes | |
| Phone | string? | 15 | Yes | |
| Fax | string? | 20 | Yes | |
| Email | string? | 255 | Yes | |
| IPAddress | string? | max | Yes | |
| IPDescription | string? | max | Yes | |
| GeoLocations | string? | max | Yes | |
| GeoRadius | double? | | Yes | |
| ABN | string? | 11 | Yes | |
| HPIONumber | string? | 20 | Yes | |
| HPIOStatus | string? | max | Yes | |
| QuarterlyGroupId | Guid? | | Yes | |
| AfterHoursServiceId | Guid? | | Yes | |
| EnablePasswordAging | bool? | | Yes | |
| PasswordAging | int? | | Yes | |
| ActiveDirectoryEnabled | bool | | No | false |
| TenantId | Guid? | | Yes | |
| ENRMCActivationDate | DateTime? | | Yes | |
| SeqNumberChart | long? | | Yes | |
| Profit | bool? | | Yes | |
| Region | string? | 200 | Yes | |
| SourceFacilityId | Guid? | | Yes | |
| Guidelines | string? | max | Yes | |
| PDSActivationDate | DateTime? | | Yes | |
| SSOOption | string? | 20 | Yes | |
| EnableTrustedSSO | bool? | | Yes | |
| CreatedDate | DateTime? | | Yes | |
| CreatedBy | Guid? | | Yes | |
| LastUpdatedDate | DateTime? | | Yes | |
| LastUpdatedBy | Guid? | | Yes | |
| ClusteredKey | long | | No | |

NAV PROPS: ICollection<Section>, ICollection<UserFacility>, ICollection<DoseRound>, ICollection<FacilityDoseConfig>, ICollection<FacilityDoseFilterConfig>, ICollection<FacilityBulkPackGenerateRange>, ICollection<BESTtrackFacilityConfig>, ICollection<WeeklyBulkRun>, ICollection<HomeCareBulkDoseRoundGenerateRange>, ICollection<S8DestructionDrug>, ICollection<S8DestructionRequest>, ICollection<BESTMEDSupplyPharmacy>

---

## Section : AuditableEntity
Child of Facility (1 Facility → many Sections)

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid? | | Yes | FK→Facility |
| Name | string? | max | Yes |
| FredCode | string? | 20 | Yes |
| Phone | string? | 20 | Yes |
| Fax | string? | 20 | Yes |
| Email | string? | max | Yes |
| IsActive | bool | | No (true) |
| ClusteredKey | long | | No |

---

## UserFacility : AuditableEntity
M2M: User ↔ Facility with permissions

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| UserId | Guid | | No | FK→User |
| FacilityId | Guid | | No | FK→Facility |
| DoctorStatus | string? | 20 | Yes |
| AccessMode | string? | 20 | Yes |
| AllResidentAccess | bool? | | Yes |
| UserPermissions | int? | | Yes |
| MedicationTrackingLocation | string? | 200 | Yes |
| BESTtrackPermission | long? | | Yes |
| IsBESTtrackOwner | bool? | | Yes |
| ConfirmationStatus | string? | 20 | Yes |
| ConfirmationRequiredDate | DateTime? | | Yes |
| ConfirmedBy | Guid? | | Yes |
| ConfirmedDate | DateTime? | | Yes |
| ClusteredKey | long | | No |

---

## DoseRound
FK→ Facility.Id, Section.Id, User.Id (OfflineBy)

| Column | C# Type | Nullable | Default |
|--------|---------|---------|---------|
| Id | Guid | No | newid() |
| FacilityId | Guid | No | |
| SectionId | Guid? | Yes | |
| DoseTime | DateTime | No | |
| StartOfRound | DateTime | No | |
| EndOfRound | DateTime | No | |
| DoseOrder | int? | Yes | |
| SpecialRound | bool? | Yes | |
| RoundComplete | bool? | Yes | |
| Notification | bool? | Yes | |
| RunDate | DateTime? | Yes | |
| ProcessId | Guid? | Yes | |
| IsOffline | bool? | Yes | false |
| OfflineDate | DateTime? | Yes | |
| OfflineBy | Guid? | Yes | |
| MissedOfflineSync | bool? | Yes | |
| WarningSent | int? | Yes | |
| ClusteredKey | long | No | |

NAV PROPS: ICollection<ResidentDose>

---

## FacilityDoseConfig : AuditableEntity
Dose time configuration per Facility

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid | | No | FK→Facility |
| DoseTime | TimeOnly? | | Yes |
| SpecialRound | bool? | | Yes |
| StartOfRound | int? | | Yes |
| EndOfRound | int? | | Yes |
| DoseOrder | int? | | Yes |
| DoseTimeLabel | string? | 30 | Yes |
| SecondaryDoseLabel | string? | 30 | Yes |
| MealColour | string? | 30 | Yes |
| ClusteredKey | long | | No |

---

## FacilityDoseFilterConfig : AuditableEntity (partial — no LastUpdated)
Dose filter/category config per Facility. Has CreatedDate/CreatedBy only.

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid | | No | FK→Facility |
| CategoryName | string? | 100 | Yes |
| CategoryOrder | int? | | Yes |
| CreatedDate | DateTime? | | Yes |
| CreatedBy | Guid? | | Yes |
| ClusteredKey | long | | No |

---

## FacilityBulkPackGenerateRange
Pack generation schedule per Facility. No audit cols.

| Column | C# Type | MaxLen | Nullable | Default |
|--------|---------|--------|---------|---------|
| Id | Guid | | No | newid() |
| FacilityId | Guid | | No | FK→Facility |
| PharmacyId | Guid? | | Yes | |
| PackStartDate | DateTime | | No | |
| PackEndDate | DateTime | | No | |
| FacilityDay1 | string? | 10 | Yes | |
| PackDayOffset | int? | | Yes | |
| RequestGeneratedDate | DateTime | | No | |
| IsMissed | bool | | No | false |
| ClusteredKey | long | | No | |

---

## BESTtrackFacilityConfig
S8/controlled drug tracking config per Facility. No audit cols (has LastUpdatedBy/LastUpdateDateUTC only).

| Column | C# Type | MaxLen | Nullable | Default |
|--------|---------|--------|---------|---------|
| Id | Guid | | No | |
| FacilityId | Guid | | No | FK→Facility |
| TimeOut | int | | No | |
| IsDualSign | bool | | No | |
| CheckOutTime | int | | No | 8 |
| PromptsNumber | int | | No | 1 |
| StockTakePeriod | int | | No | 0 |
| StockTakeFrequency | string? | 20 | Yes | |
| StockTakeS8Period | int | | No | 0 |
| StockTakeS8Frequency | string? | 20 | Yes | |
| NotificationMethod | string? | 1000 | Yes | |
| NotificationGraceTime | int? | | Yes | |
| DiscrepancyNotifications | bool | | No | false |
| UsersNotify | string? | 1000 | Yes | |
| EnablePrescriberInstructions | bool? | | Yes | |
| EnableTransferFromBook | bool | | No | false |
| TrackS4DMedications | bool | | No | true |
| CommentChkOutMandatory | bool? | | Yes | false |
| DestroyMedication | bool | | No | true |
| EnableBalanceBottles | bool | | No | false |
| EnablePrescriberNameMandatory | bool | | No | false |
| ReEnterPINForDualSigning | bool? | | Yes | |
| PasswordAndPINTransaction | bool? | | Yes | |
| EnableBESTTrackDoseFormUnit | bool? | | Yes | |
| DisableDualSignIndividualStockCount | bool? | | Yes | |
| BCPChartNotifications | bool? | | Yes | |
| LastUpdatedBy | Guid | | No | |
| LastUpdateDateUTC | DateTime | | No | |
| ClusteredKey | long | | No | |

---

## WeeklyBulkRun
FK→ Facility.Id, Section.Id. No audit cols.

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid | | No |
| SectionId | Guid? | | Yes |
| PharmacyId | Guid? | | Yes |
| RunDate | DateTime | | No |
| PackStartDate | DateTime | | No |
| PackEndDate | DateTime | | No |
| Status | bool | | No |
| RequestType | int | | No |
| FailReason | string? | max | Yes |
| ResidentsProcessed | int? | | Yes |
| ResidentsRejected | int? | | Yes |
| RejectedResidentIds | string? | max | Yes |
| ProcessId | Guid? | | Yes |
| ClusteredKey | long | | No |

---

## HomeCareBulkDoseRoundGenerateRange
FK→ Facility.Id. No audit cols.

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid | | No |
| PharmacyId | Guid | | No |
| DoseRoundStartDate | DateTime | | No |
| DoseRoundEndDate | DateTime | | No |
| FacilityDay1 | string | 5 | No |
| DoseRoundDayOffset | int | | No |
| RequestGeneratedDate | DateTime | | No |
| ClusteredKey | long | | No |

---

## S8DestructionDrug
FK→ Facility.Id, Resident.Id, Section.Id, User.Id (DestroyedBy)

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid | | No |
| PharmacyId | Guid? | | Yes |
| ResidentId | Guid? | | Yes |
| ResidentName | string? | 120 | Yes |
| SectionId | Guid? | | Yes |
| DrugId | Guid? | | Yes |
| MedicineName | string? | max | Yes |
| Strength | string? | 20 | Yes |
| Quantity | decimal? | | Yes |
| Note | string? | max | Yes |
| Destroyed | bool | | No (false) |
| DestroyedBy | Guid? | | Yes |
| DestroyedDate | DateTime? | | Yes |
| ClusteredKey | long | | No |

---

## S8DestructionRequest
FK→ Facility.Id, User.Id (ProcessedBy)

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid | | No |
| PharmacyId | Guid? | | Yes |
| RequestType | string | 20 | No |
| RequestedBy | Guid | | No |
| RequestedDate | DateTime? | | Yes |
| ProcessedBy | Guid? | | Yes |
| ActionDate | DateTime? | | Yes |
| ActionTime | int? | | Yes |

NO ClusteredKey. NO audit cols.

---

## ReportMedicationSummary
FK→ Facility.Id

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityId | Guid? | | Yes |
| FacilityName | string? | 100 | Yes |
| ReportDate | DateTime? | | Yes |
| Period | string? | 50 | Yes |
| ReportCategory | int? | | Yes |
| MedicationCategory | string? | 100 | Yes |
| NumberOfResident | int? | | Yes |
| TotalNumberOfResidents | int? | | Yes |
| Area | string? | max | Yes |
| ClusteredKey | long | | No |

NAV PROPS: ICollection<ReportMedicationSummarySection>

---

## ReportMedicationSummarySection
FK→ ReportMedicationSummary.Id, Section.Id. No audit cols.

| Column | C# Type |
|--------|---------|
| Id | Guid |
| ReportMedicationSummaryId | Guid | FK→ReportMedicationSummary
| SectionId | Guid | FK→Section
| ClusteredKey | long |

---

## UTILog
FK→ Facility.Id (as FacilityID). No audit cols.

| Column | C# Type | MaxLen | Nullable |
|--------|---------|--------|---------|
| Id | Guid | | No |
| FacilityID | Guid | | No |
| FacilityName | string | 100 | No |
| AlertType | string | 30 | No |
| AlertDateTime | DateTime | | No |
| PrescriberNo | string | 40 | No |
| PrescriberLastName | string | 100 | No |
| PrescriberFirstName | string | 100 | No |
| ClinicalDecision | string | 15 | No |
| ResidentAgeInYears | int? | | Yes |
| ResidentGender | string | 20 | No |
| Indication | string? | 100 | Yes |
| OrganismCultureSensitivity | string? | 50 | Yes |
| RecommendedDuration | string | 200 | No |
| RecommendedDose | string? | 200 | Yes |
| RecommendedAgent | string | 100 | No |
| ReasonForNotChangingThePrescription | string? | 150 | Yes |
| PrescribedMedication | string? | 250 | Yes |
| PrescribedDuration | string? | 100 | Yes |
| ClusteredKey | long | | No |

---

## REPOSITORIES NEEDED
| Interface | Aggregate Root | Notes |
|-----------|---------------|-------|
| IFacilityRepository | Facility | Include Section, UserFacility, FacilityDoseConfig, BESTtrackFacilityConfig |
| IDoseRoundRepository | DoseRound | Include ResidentDose children |
| IWeeklyBulkRunRepository | WeeklyBulkRun | Standalone |
| IS8DestructionRepository | S8DestructionRequest | Include S8DestructionDrug (query by FacilityId) |
| IReportMedicationSummaryRepository | ReportMedicationSummary | Include ReportMedicationSummarySection |
| IUTILogRepository | UTILog | Standalone, read-heavy |