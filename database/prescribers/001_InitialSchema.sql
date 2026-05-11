-- ============================================================================
-- BestMed PrescriberService — Initial Schema
-- Database: sqldb-bmp-prescribers-{env}
-- Run against: sqls-bmp-{env}.database.windows.net
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Prescriber')
BEGIN
    CREATE TABLE [dbo].[Prescriber]
    (
        [Id]                                UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [PrescriberName]                    NVARCHAR(100)     NOT NULL,
        [PrescriberCode]                    NVARCHAR(20)      NOT NULL,
        [Email]                             NVARCHAR(255)     NULL,
        [Phone]                             NVARCHAR(20)      NULL,
        [Fax]                               NVARCHAR(20)      NULL,
        [OutOfHours]                        NVARCHAR(20)      NULL,
        [Address1]                          NVARCHAR(100)     NULL,
        [Address2]                          NVARCHAR(100)     NULL,
        [Suburb]                            NVARCHAR(50)      NULL,
        [State]                             NVARCHAR(10)      NULL,
        [Postcode]                          NVARCHAR(4)       NULL,
        [Country]                           NVARCHAR(50)      NULL,
        [FirstName]                         NVARCHAR(50)      NULL,
        [LastName]                          NVARCHAR(50)      NULL,
        [PINHash]                           NVARCHAR(100)     NULL,
        [PINSalt]                           NVARCHAR(100)     NULL,
        [EmailNotifications]                INT               NULL,
        [MobileNumber]                      NVARCHAR(20)      NULL,
        [HPIINumber]                        NVARCHAR(20)      NULL,
        [HPIIStatus]                        NVARCHAR(50)      NULL,
        [AHPRANumber]                       NVARCHAR(13)      NULL,
        [PinAcknowledge]                    BIT               NOT NULL DEFAULT 0,
        [LicenseNumber]                     VARCHAR(20)       NULL,
        [ClusteredKey]                      BIGINT            IDENTITY(1,1) NOT NULL,
        [Qualification]                     NVARCHAR(30)      NULL,
        [DefaultMimsSeverityLevel]          NVARCHAR(100)     NULL,
        [DefaultMimsDocumentationLevel]     NVARCHAR(100)     NULL,
        [EnableMimsDrugInteractionChecking] BIT               NOT NULL DEFAULT 0,
        [IseRxUserAccessAgreementAccepted]  BIT               NULL,
        [ERxUserAccessAgreementAcceptedDate] DATETIME2(7)     NULL,
        [ERxUserAccessAgreementVersion]     NVARCHAR(200)     NULL,
        [ERxEntityId]                       NVARCHAR(200)     NULL,
        [PreferredName]                     NVARCHAR(50)      NULL,

        CONSTRAINT [PK_Prescriber] PRIMARY KEY CLUSTERED ([Id])
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Prescriber_ClusteredKey] ON [dbo].[Prescriber] ([ClusteredKey]);
    CREATE NONCLUSTERED INDEX [IX_Prescriber_PrescriberCode] ON [dbo].[Prescriber] ([PrescriberCode]);
    CREATE NONCLUSTERED INDEX [IX_Prescriber_Email] ON [dbo].[Prescriber] ([Email]);
    CREATE NONCLUSTERED INDEX [IX_Prescriber_AHPRANumber] ON [dbo].[Prescriber] ([AHPRANumber]);
END
GO
