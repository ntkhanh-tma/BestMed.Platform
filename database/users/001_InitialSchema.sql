-- ============================================================================
-- BestMed UserService — Initial Schema
-- Database: sqldb-bmp-users-{env}
-- Run against: sqls-bmp-{env}.database.windows.net
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'User')
BEGIN
    CREATE TABLE [dbo].[User]
    (
        [Id]                              UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWSEQUENTIALID(),
        [PasswordHash]                    NVARCHAR(100)      NOT NULL,
        [Salt]                            NVARCHAR(100)      NULL,
        [UserId]                          NVARCHAR(255)      NULL,
        [PIN]                             NVARCHAR(6)        NULL,
        [IsActive]                        BIT                NULL,
        [ResetPassword]                   BIT                NULL,
        [PasswordResetCode]               UNIQUEIDENTIFIER   NULL,
        [Role]                            UNIQUEIDENTIFIER   NOT NULL,
        [Type]                            NVARCHAR(10)       NOT NULL,
        [FirstName]                       NVARCHAR(50)       NULL,
        [LastName]                        NVARCHAR(50)       NULL,
        [Salutation]                      NVARCHAR(50)       NULL,
        [JobTitle]                        NVARCHAR(50)       NULL,
        [ContactNumber]                   NVARCHAR(50)       NULL,
        [LockToIP]                        BIT                NULL,
        [IsTermsAndConditionsAccepted]    BIT                NOT NULL DEFAULT 0,
        [TermsAndConditionsAcceptedDate]  DATETIME2(7)       NULL,
        [Email]                           NVARCHAR(255)      NULL,
        [LoginFailedCount]                INT                NOT NULL DEFAULT 0,
        [LastLogin]                       DATETIME2(7)       NULL,
        [Status]                          NVARCHAR(20)       NULL,
        [LastSectionUsed]                 UNIQUEIDENTIFIER   NULL,
        [LastFacilityUsed]                UNIQUEIDENTIFIER   NULL,
        [CreatedDate]                     DATETIME2(7)       NULL,
        [CreatedBy]                       UNIQUEIDENTIFIER   NULL,
        [LastUpdatedDate]                 DATETIME2(7)       NULL,
        [LastUpdatedBy]                   UNIQUEIDENTIFIER   NULL,
        [PrescriberId]                    UNIQUEIDENTIFIER   NULL,
        [IsBHSStaff]                      BIT                NOT NULL DEFAULT 0,
        [PasswordLastUpdated]             DATETIME2(7)       NULL,
        [IsExternalLogin]                 BIT                NOT NULL DEFAULT 0,
        [ExternalUserId]                  NVARCHAR(500)      NULL,
        [IsBESTmedLogin]                  BIT                NOT NULL DEFAULT 0,
        [LoginIdChangedCount]             INT                NOT NULL DEFAULT 0,
        [SignOutRule]                      TINYINT            NULL,
        [AHPRANumber]                     NVARCHAR(13)       NULL,
        [LastMedicationHistoryTab]        SMALLINT           NULL,
        [PinFailedCount]                  INT                NULL,
        [EmailConfirmed]                  BIT                NOT NULL DEFAULT 0,
        [LockoutEnabled]                  BIT                NOT NULL DEFAULT 0,
        [LockoutEnd]                      DATETIMEOFFSET(7)  NULL,
        [PhoneNumberConfirmed]            BIT                NOT NULL DEFAULT 0,
        [SecurityStamp]                   NVARCHAR(MAX)      NOT NULL,
        [ConcurrencyStamp]               NVARCHAR(MAX)      NULL,
        [NormalizedEmail]                 NVARCHAR(255)      NULL,
        [NormalizedUserId]                NVARCHAR(255)      NULL,
        [TwoFactorEnabled]               BIT                NOT NULL DEFAULT 0,
        [IsProxyAccount]                  BIT                NULL,
        [UserQualifications]              NVARCHAR(MAX)      NULL,
        [DeviceRegistrationPinFailedCount] INT              NULL,
        [HPIINumber]                      NVARCHAR(20)       NULL,
        [HPIIStatus]                      NVARCHAR(50)       NULL,
        [IsReadOnlyAccess]                BIT                NOT NULL DEFAULT 0,
        [CustomRoleCheckSum]              NVARCHAR(MAX)      NULL,
        [LockTime]                        DATETIME2(7)       NULL,
        [IntegrationId]                   NVARCHAR(50)       NULL,
        [IntegrationSystem]               NVARCHAR(100)      NULL,
        [PinDualFailedCount]              INT                NULL,
        [PreferredName]                   NVARCHAR(50)       NULL,

        CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_User_Email] ON [dbo].[User] ([Email]);
    CREATE NONCLUSTERED INDEX [IX_User_UserId] ON [dbo].[User] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_User_NormalizedUserId] ON [dbo].[User] ([NormalizedUserId]);
    CREATE NONCLUSTERED INDEX [IX_User_IsActive] ON [dbo].[User] ([IsActive]) INCLUDE ([Email], [FirstName], [LastName]);
END
GO
