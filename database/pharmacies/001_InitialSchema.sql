-- ============================================================
-- Pharmacy Service — Initial Schema
-- Database: sqldb-bmp-pharmacies-dev
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Pharmacy')
BEGIN
	CREATE TABLE [dbo].[Pharmacy] (
		[Id]                                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[Name]                                  NVARCHAR(100)    NOT NULL,
		[WarehouseId]                           UNIQUEIDENTIFIER NULL,
		[StateTimeZoneId]                       UNIQUEIDENTIFIER NULL,
		[Active]                                BIT              NOT NULL DEFAULT 1,
		[PharmacyType]                          INT              NULL DEFAULT 0,
		[Address1]                              NVARCHAR(100)    NULL,
		[Address2]                              NVARCHAR(100)    NULL,
		[Suburb]                                NVARCHAR(50)     NULL,
		[State]                                 NVARCHAR(30)     NULL,
		[PostCode]                              NVARCHAR(4)      NULL,
		[Country]                               NVARCHAR(30)     NULL,
		[ShippingAddress1]                      NVARCHAR(100)    NULL,
		[ShippingAddress2]                      NVARCHAR(100)    NULL,
		[ShippingState]                         NVARCHAR(30)     NULL,
		[ShippingSuburb]                        NVARCHAR(50)     NULL,
		[ShippingPostCode]                      NVARCHAR(4)      NULL,
		[IsUseBillingAddress]                   BIT              NOT NULL DEFAULT 1,
		[BillingName]                           NVARCHAR(200)    NULL,
		[BillingContactId]                      UNIQUEIDENTIFIER NULL,
		[ContactName]                           NVARCHAR(50)     NULL,
		[Phone]                                 NVARCHAR(20)     NULL,
		[Fax]                                   NVARCHAR(20)     NULL,
		[Email]                                 NVARCHAR(255)    NULL,
		[OutOfHours]                            NVARCHAR(20)     NULL,
		[ABN]                                   NVARCHAR(11)     NULL,
		[PharmacyApprovalNumber]                NVARCHAR(20)     NULL,
		[HPIONumber]                            NVARCHAR(20)     NULL,
		[HPIOStatus]                            NVARCHAR(MAX)    NULL,
		[HasPackingFacility]                    BIT              NULL,
		[S8DrugPackingAllowed]                  BIT              NULL,
		[IsMultiSite]                           BIT              NOT NULL DEFAULT 0,
		[IsInDispenseMigration]                 BIT              NOT NULL DEFAULT 0,
		[LastDispenseMigrationDate]             DATETIME         NULL,
		[AutoSyncDispenseResident]              BIT              NOT NULL DEFAULT 1,
		[DispenseSystemType]                    INT              NULL,
		[FinancialType]                         INT              NULL,
		[MinValue]                              DECIMAL(18,2)    NULL,
		[MaxValue]                              DECIMAL(18,2)    NULL,
		[Discount]                              DECIMAL(18,2)    NULL,
		[DaysInvoiceTerms]                      INT              NULL,
		[EnablePasswordAging]                   BIT              NULL,
		[PasswordAging]                         INT              NULL,
		[WorkingDays]                           NVARCHAR(14)     NULL,
		[GeoLocations]                          NVARCHAR(MAX)    NULL,
		[GeoRadius]                             FLOAT            NULL,
		[IPAddress]                             NVARCHAR(MAX)    NULL,
		[IPDescription]                         NVARCHAR(500)    NULL,
		[XMLUserName]                           NVARCHAR(100)    NULL DEFAULT '',
		[XMLUserPassword]                       NVARCHAR(150)    NULL DEFAULT '',
		[DropboxToken]                          NVARCHAR(100)    NULL,
		[FredNXTAccessToken]                    NVARCHAR(200)    NULL,
		[LastFredNXTResidentSyncDate]           DATETIME         NULL,
		[LastFredNXTScriptSyncDate]             DATETIME         NULL,
		[LastFredNXTPrescriberSyncDate]         DATETIME         NULL,
		[BCPChartGenerationEnabled]             BIT              NOT NULL DEFAULT 0,
		[BCPSigningSheetGenerationEnabled]      BIT              NOT NULL DEFAULT 0,
		[EnablePackScheduleAPI]                 BIT              NULL,
		[PackScheduleAPIEnabledDate]            DATETIME         NULL,
		[EnableDashboard]                       BIT              NOT NULL DEFAULT 1,
		[EnableDistributedPacking]              BIT              NULL,
		[IsBestpackCore]                        BIT              NULL,
		[HomeCareExclude]                       BIT              NOT NULL DEFAULT 0,
		[AllowPreferredBrandSubstitution]       BIT              NULL,
		[AllowAllClaims]                        BIT              NULL,
		[IsAutoFacilityReport]                  BIT              NULL,
		[AutoFacilityReportGenerateDate]        INT              NULL,
		[IsAutoComplianceReport]                BIT              NULL,
		[IsAutoBulkMedChart]                    BIT              NULL,
		[HasCPIClause]                          BIT              NOT NULL DEFAULT 0,
		[EnableAddMoveResidentViaBpack]         BIT              NULL,
		[ProgrammeJoinedDate]                   DATE             NULL,
		[Tier]                                  NVARCHAR(20)     NULL,
		[MedicationTrackingReturnedReason]      NVARCHAR(MAX)    NULL,
		[S8DestructionEmail]                    NVARCHAR(150)    NULL,
		[UpdateNrmcCompliantForAudit]           BIT              NULL,
		[ArchiveFredUsedRepeat]                 BIT              NULL,
		[PriceModel]                            UNIQUEIDENTIFIER NULL,
		[CheckingMachineType]                   UNIQUEIDENTIFIER NULL,
		[TermsAndConditions]                    NVARCHAR(MAX)    NULL,
		[TermsAndConditionsType]                NVARCHAR(15)     NULL,
		[ResidentsFacilityCode]                 NVARCHAR(10)     NULL,
		[ClaimName]                             NVARCHAR(25)     NULL,
		[ClaimQualification]                    NVARCHAR(25)     NULL,
		[SachetRobotTypeId]                     NVARCHAR(MAX)    NULL,
		[BlisterRobotTypeId]                    NVARCHAR(MAX)    NULL,
		[YuyamaModelId]                         NVARCHAR(MAX)    NULL,
		[SachetPackingMachineInstalledDate]     DATE             NULL,
		[SachetPackingMachineAssetNumber]       NVARCHAR(50)     NULL,
		[SachetPackingMachineInstalledDateSecond] DATE           NULL,
		[SachetPackingMachineAssetNumberSecond] NVARCHAR(50)     NULL,
		[BlisterPackingMachineInstalledDate]    DATE             NULL,
		[BlisterPackingMachineAssetNumber]      NVARCHAR(50)     NULL,
		[BlisterPackingMachineInstalledDateSecond] DATE          NULL,
		[BlisterPackingMachineAssetNumberSecond] NVARCHAR(50)    NULL,
		[CheckingMachineInstalledDate]          DATE             NULL,
		[CheckingMachineAssetNumber]            NVARCHAR(50)     NULL,
		[CheckingMachineInstalledDateSecond]    DATE             NULL,
		[CheckingMachineAssetNumberSecond]      NVARCHAR(50)     NULL,
		[CheckingSoftwareMaintenanceFee]        DECIMAL(18,2)    NULL,
		[ResidentFacilityFeeLessThanOrEqualTo1000] DECIMAL(18,2) NULL,
		[ResidentFacilityFeeGreaterThan1000]    DECIMAL(18,2)    NULL,
		[BESTpackCommunityFee]                  DECIMAL(18,2)    NULL,
		[BESTdoctorResidentFee]                 DECIMAL(18,2)    NULL,
		[BESTpackMonthlySupportFee]             DECIMAL(18,2)    NULL,
		[BESTpackDoctorIntegrationFee]          DECIMAL(18,2)    NULL,
		[OtherInformation]                      NVARCHAR(500)    NULL,
		[ClusteredKey]                          BIGINT           IDENTITY(1,1) NOT NULL,
		[CreatedDate]                           DATETIME         NULL,
		[CreatedBy]                             UNIQUEIDENTIFIER NULL,
		[LastUpdatedDate]                       DATETIME         NULL,
		[LastUpdatedBy]                         UNIQUEIDENTIFIER NULL,
		CONSTRAINT [PK_Pharmacy] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_Pharmacy_ClusteredKey] CLUSTERED ([ClusteredKey])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Facility')
BEGIN
	CREATE TABLE [dbo].[Facility] (
		[Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PharmacyId]    UNIQUEIDENTIFIER NULL,
		[ClusteredKey]  BIGINT           IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_Facility] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_Facility_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_Facility_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserPharmacy')
BEGIN
	CREATE TABLE [dbo].[UserPharmacy] (
		[Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[UserId]          UNIQUEIDENTIFIER NOT NULL,
		[PharmacyId]      UNIQUEIDENTIFIER NOT NULL,
		[ClusteredKey]    BIGINT           IDENTITY(1,1) NOT NULL,
		[CreatedDate]     DATETIME         NULL,
		[CreatedBy]       UNIQUEIDENTIFIER NULL,
		[LastUpdatedDate] DATETIME         NULL,
		[LastUpdatedBy]   UNIQUEIDENTIFIER NULL,
		CONSTRAINT [PK_UserPharmacy] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_UserPharmacy_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_UserPharmacy_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BCPSetting')
BEGIN
	CREATE TABLE [dbo].[BCPSetting] (
		[Id]                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PharmacyId]          UNIQUEIDENTIFIER NOT NULL,
		[DropboxToken]        NVARCHAR(100)    NULL,
		[OneDriveToken]       NVARCHAR(MAX)    NULL,
		[OneDriveTokenExpired] DATETIME        NULL,
		[OneDriveExpired]     DATETIME         NULL,
		[ClusteredKey]        BIGINT           IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_BCPSetting] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_BCPSetting_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_BCPSetting_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BESTMEDSupplyPharmacy')
BEGIN
	CREATE TABLE [dbo].[BESTMEDSupplyPharmacy] (
		[Id]                                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PharmacyId]                        UNIQUEIDENTIFIER NOT NULL,
		[FacilityId]                        UNIQUEIDENTIFIER NOT NULL,
		[FacilityCode]                      NVARCHAR(20)     NOT NULL,
		[IsActive]                          BIT              NOT NULL,
		[BulkPackDayOffset]                 INT              NULL,
		[NonPackDeliveryDay]                NVARCHAR(10)     NULL,
		[S8DrugPackingAllowed]              BIT              NULL,
		[NRMCCompliantDefault]              BIT              NULL,
		[VariablePackType]                  NVARCHAR(20)     NULL,
		[PrnPackType]                       NVARCHAR(20)     NULL,
		[ShortCoursePackType]               NVARCHAR(20)     NULL,
		[CytotoxicPackType]                 NVARCHAR(20)     NULL,
		[AntibioticsPackType]               NVARCHAR(20)     NULL,
		[AntimicrobialPackTypeShortCourse]  NVARCHAR(20)     NULL,
		[S4DPackType]                       NVARCHAR(10)     NOT NULL DEFAULT 'NP',
		[PackForm]                          NVARCHAR(20)     NULL,
		[DualPackingEnabled]                BIT              NULL,
		[DefaultPackHeaderType]             NVARCHAR(20)     NULL,
		[DefaultPackingLocation]            NVARCHAR(20)     NULL,
		[S8ToBePackedSeparately]            BIT              NULL,
		[CytotoxicToBePackedSeparately]     BIT              NULL,
		[CytostaticToBePackedSeparately]    BIT              NULL,
		[VariableToBePackedSeparately]      BIT              NULL,
		[S4DToBePackedSeparately]           BIT              NULL,
		[DoNotCrushToBePackedSeparately]    BIT              NULL,
		[PackDNCandRegularInSamePackRoll]   BIT              NULL,
		[ShowMedicationOnFrontOfThePack]    BIT              NULL,
		[IsBlisterStartFromBottom]          BIT              NULL,
		[PackedPRNExpiryPeriodInMonths]     INT              NULL,
		[RegularExpiry]                     INT              NULL,
		[S8Expiry]                          INT              NULL,
		[SachetPrintTime]                   INT              NULL,
		[CommunitySachetLayout]             BIT              NULL,
		[UtiliseAdditionalBulkRolls]        BIT              NULL,
		[RobotTypeId]                       UNIQUEIDENTIFIER NULL,
		[DistributedPackingRobotTypeId]     UNIQUEIDENTIFIER NULL,
		[YuyamaModelId]                     UNIQUEIDENTIFIER NULL,
		[DistributedPackingYuyamaModelId]   UNIQUEIDENTIFIER NULL,
		[AllowAliasing]                     BIT              NULL,
		[FacilityLiveDate]                  DATETIME         NULL,
		[ForceDownload]                     BIT              NULL,
		[ForceMedChart]                     BIT              NULL,
		[MedChangeCutoffTime]               NVARCHAR(10)     NULL,
		[OnlineOrderingCutoffTime]          NVARCHAR(10)     NULL,
		[PrintClaimsSeparately]             BIT              NULL,
		[S8ReportDay]                       NVARCHAR(10)     NULL,
		[PharmacyEyeDropDeliveryDay]        NVARCHAR(10)     NULL,
		[EmergencyStockPatientNumber]       NVARCHAR(10)     NULL,
		[EnableMedicationTracking]          BIT              NULL,
		[MedicationTrackingDispatchLocation] NVARCHAR(250)   NULL,
		[ClusteredKey]                      BIGINT           IDENTITY(1,1) NOT NULL,
		[CreatedDate]                       DATETIME         NULL,
		[CreatedBy]                         UNIQUEIDENTIFIER NULL,
		[LastUpdatedDate]                   DATETIME         NULL,
		[LastUpdatedBy]                     UNIQUEIDENTIFIER NULL,
		CONSTRAINT [PK_BESTMEDSupplyPharmacy] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_BESTMEDSupplyPharmacy_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_BESTMEDSupplyPharmacy_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SupplyPharmacy')
BEGIN
	CREATE TABLE [dbo].[SupplyPharmacy] (
		[Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[Name]            NVARCHAR(100)    NOT NULL,
		[FacilityId]      UNIQUEIDENTIFIER NOT NULL,
		[IsActive]        BIT              NOT NULL DEFAULT 1,
		[Address1]        NVARCHAR(100)    NULL,
		[Address2]        NVARCHAR(100)    NULL,
		[Suburb]          NVARCHAR(50)     NULL,
		[State]           NVARCHAR(50)     NULL,
		[PostCode]        NVARCHAR(4)      NULL,
		[Country]         NVARCHAR(30)     NULL,
		[Phone]           NVARCHAR(20)     NULL,
		[Fax]             NVARCHAR(20)     NULL,
		[Email]           NVARCHAR(255)    NULL,
		[OutOfHours]      NVARCHAR(20)     NULL,
		[IPAddress]       NVARCHAR(MAX)    NULL,
		[CreatedDate]     DATETIME         NOT NULL,
		[CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
		[LastUpdatedDate] DATETIME         NOT NULL,
		[LastUpdatedBy]   UNIQUEIDENTIFIER NOT NULL,
		[ClusteredKey]    BIGINT           IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_SupplyPharmacy] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_SupplyPharmacy_ClusteredKey] CLUSTERED ([ClusteredKey])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SupplyPharmacySection')
BEGIN
	CREATE TABLE [dbo].[SupplyPharmacySection] (
		[Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FacilityId]      UNIQUEIDENTIFIER NOT NULL,
		[PharmacyId]      UNIQUEIDENTIFIER NOT NULL,
		[SectionId]       UNIQUEIDENTIFIER NOT NULL,
		[SectionCode]     NVARCHAR(20)     NOT NULL,
		[ClusteredKey]    BIGINT           IDENTITY(1,1) NOT NULL,
		[CreatedDate]     DATETIME         NULL,
		[CreatedBy]       UNIQUEIDENTIFIER NULL,
		[LastUpdatedDate] DATETIME         NULL,
		[LastUpdatedBy]   UNIQUEIDENTIFIER NULL,
		CONSTRAINT [PK_SupplyPharmacySection] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_SupplyPharmacySection_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_SupplyPharmacySection_SupplyPharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[SupplyPharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NonBhsUserPharmacy')
BEGIN
	CREATE TABLE [dbo].[NonBhsUserPharmacy] (
		[Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[UserId]          UNIQUEIDENTIFIER NOT NULL,
		[PharmacyId]      UNIQUEIDENTIFIER NOT NULL,
		[ClusteredKey]    BIGINT           IDENTITY(1,1) NOT NULL,
		[CreatedDate]     DATETIME         NULL,
		[CreatedBy]       UNIQUEIDENTIFIER NULL,
		[LastUpdatedDate] DATETIME         NULL,
		[LastUpdatedBy]   UNIQUEIDENTIFIER NULL,
		CONSTRAINT [PK_NonBhsUserPharmacy] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_NonBhsUserPharmacy_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_NonBhsUserPharmacy_SupplyPharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[SupplyPharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PackRequest')
BEGIN
	CREATE TABLE [dbo].[PackRequest] (
		[Id]                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PharmacyId]            UNIQUEIDENTIFIER NOT NULL,
		[FacilityId]            UNIQUEIDENTIFIER NOT NULL,
		[SectionId]             UNIQUEIDENTIFIER NULL,
		[RequestNumber]         NVARCHAR(20)     NOT NULL,
		[PackRequestDate]       DATETIME         NOT NULL,
		[PackRequestBy]         UNIQUEIDENTIFIER NOT NULL,
		[PackFrom]              DATETIME         NULL,
		[PackTo]                DATETIME         NULL,
		[PackRequestType]       INT              NOT NULL,
		[Status]                INT              NOT NULL,
		[WarehouseStatus]       INT              NULL,
		[Generatedby]           NVARCHAR(1)      NOT NULL,
		[PackDocumentId]        UNIQUEIDENTIFIER NULL,
		[BillDocumentId]        UNIQUEIDENTIFIER NULL,
		[ProcessId]             UNIQUEIDENTIFIER NULL,
		[IsVerified]            BIT              NULL,
		[BillStatus]            BIT              NULL,
		[RobotTypeId]           UNIQUEIDENTIFIER NULL,
		[Packer]                NVARCHAR(255)    NULL,
		[VMCReverseDate]        DATETIME         NULL,
		[VMCReverseBy]          UNIQUEIDENTIFIER NULL,
		[PackRequestLocationId] INT              NULL,
		CONSTRAINT [PK_PackRequest] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_PackRequest_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PackResidentRoll')
BEGIN
	CREATE TABLE [dbo].[PackResidentRoll] (
		[Id]                                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PackRequestId]                     UNIQUEIDENTIFIER NOT NULL,
		[ResidentId]                        UNIQUEIDENTIFIER NOT NULL,
		[PackType]                          INT              NOT NULL,
		[ActiveMedProfileId]                UNIQUEIDENTIFIER NOT NULL,
		[PrevMedProfileId]                  UNIQUEIDENTIFIER NULL,
		[ResidentPackFrom]                  DATETIME         NULL,
		[ResidentPackTo]                    DATETIME         NULL,
		[BillPackQtyDays]                   INT              NULL,
		[IsS8]                              BIT              NULL,
		[IsAccepted]                        BIT              NULL,
		[RejectReason]                      NVARCHAR(255)    NULL,
		[DoNotCharge]                       BIT              NULL,
		[VerifiedBy]                        UNIQUEIDENTIFIER NULL,
		[VerifiedDate]                      DATETIME         NULL,
		[RollNumber]                        NVARCHAR(4)      NULL,
		[IsIssued]                          BIT              NOT NULL DEFAULT 0,
		[Disposed]                          BIT              NULL,
		[ReturnedtoPharmacy]                BIT              NULL,
		[Discharged]                        BIT              NULL,
		[MedicationTrackingActionId]        UNIQUEIDENTIFIER NULL,
		[TrackedBy]                         UNIQUEIDENTIFIER NULL,
		[TrackedDate]                       DATETIME         NULL,
		[MedicationTrackingLastLocation]    NVARCHAR(200)    NULL,
		[MedicationTrackingCurrentLocation] NVARCHAR(200)    NULL,
		[ClusteredKey]                      BIGINT           IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_PackResidentRoll] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_PackResidentRoll_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_PackResidentRoll_PackRequest] FOREIGN KEY ([PackRequestId]) REFERENCES [dbo].[PackRequest]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PackResidentMed')
BEGIN
	CREATE TABLE [dbo].[PackResidentMed] (
		[Id]                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PackResidentRollId]    UNIQUEIDENTIFIER NOT NULL,
		[MedicineId]            UNIQUEIDENTIFIER NOT NULL,
		[DrugId]                UNIQUEIDENTIFIER NOT NULL,
		[DrugType]              NVARCHAR(20)     NOT NULL,
		[PRNDoses]              INT              NULL,
		[BillPackQtyWhole]      DECIMAL(18,2)    NULL,
		[BillPackQtyFractional] INT              NULL,
		[DoseQty]               DECIMAL(18,4)    NULL,
		[ClusteredKey]          BIGINT           IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_PackResidentMed] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_PackResidentMed_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_PackResidentMed_PackResidentRoll] FOREIGN KEY ([PackResidentRollId]) REFERENCES [dbo].[PackResidentRoll]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BatchReferenceFileBuilder')
BEGIN
	CREATE TABLE [dbo].[BatchReferenceFileBuilder] (
		[Id]                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PharmacyId]         UNIQUEIDENTIFIER NOT NULL,
		[DocumentId]         UNIQUEIDENTIFIER NOT NULL,
		[PackRequestNumber]  NVARCHAR(20)     NOT NULL,
		[MedicationCount]    INT              NULL,
		[CreatedDate]        DATETIME         NOT NULL,
		[CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
		CONSTRAINT [PK_BatchReferenceFileBuilder] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_BatchReferenceFileBuilder_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PharmacyInvoiceDocument')
BEGIN
	CREATE TABLE [dbo].[PharmacyInvoiceDocument] (
		[Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PharmacyId]    UNIQUEIDENTIFIER NOT NULL,
		[DocumentId]    UNIQUEIDENTIFIER NOT NULL,
		[InvoiceNumber] NVARCHAR(20)     NOT NULL,
		[InvoiceStatus] NVARCHAR(20)     NOT NULL,
		[CreatedDate]   DATETIME         NULL,
		[ClusteredKey]  BIGINT           IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_PharmacyInvoiceDocument] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_PharmacyInvoiceDocument_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_PharmacyInvoiceDocument_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QuarterlyPharmacyGroup')
BEGIN
	CREATE TABLE [dbo].[QuarterlyPharmacyGroup] (
		[Id]               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[PharmacyId]       UNIQUEIDENTIFIER NOT NULL,
		[QuarterlyGroupId] UNIQUEIDENTIFIER NOT NULL,
		[ClusteredKey]     BIGINT           IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_QuarterlyPharmacyGroup] PRIMARY KEY NONCLUSTERED ([Id]),
		INDEX [IX_QuarterlyPharmacyGroup_ClusteredKey] CLUSTERED ([ClusteredKey]),
		CONSTRAINT [FK_QuarterlyPharmacyGroup_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [dbo].[Pharmacy]([Id])
	);
END
GO
