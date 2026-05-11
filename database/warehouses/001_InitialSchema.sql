-- ============================================================================
-- BestMed WarehouseService — Initial Schema
-- Database: sqldb-bmp-warehouses-{env}
-- Run against: sqls-bmp-{env}.database.windows.net
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Warehouse')
BEGIN
    CREATE TABLE [dbo].[Warehouse]
    (
        [Id]                  UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Name]                NVARCHAR(50)      NOT NULL,
        [Address1]            NVARCHAR(100)     NULL,
        [Address2]            NVARCHAR(50)      NULL,
        [Suburb]              NVARCHAR(50)      NULL,
        [State]               NVARCHAR(50)      NULL,
        [PostCode]            NVARCHAR(6)       NULL,
        [Country]             NVARCHAR(30)      NULL,
        [ContactName]         NVARCHAR(30)      NULL,
        [Phone]               NVARCHAR(20)      NULL,
        [Fax]                 NVARCHAR(20)      NULL,
        [Email]               NVARCHAR(150)     NULL,
        [IPAddress]           NVARCHAR(MAX)     NULL,
        [CreatedDate]         DATETIME2(7)      NULL,
        [CreatedBy]           UNIQUEIDENTIFIER  NULL,
        [LastUpdatedDate]     DATETIME2(7)      NULL,
        [LastUpdatedBy]       UNIQUEIDENTIFIER  NULL,
        [XMLUserPassword]     NVARCHAR(150)     NULL,
        [XMLUserName]         NVARCHAR(100)     NULL,
        [StateTimeZoneId]     UNIQUEIDENTIFIER  NULL,
        [SachetRobotTypeId]   NVARCHAR(MAX)     NULL,
        [GeoLocations]        NVARCHAR(MAX)     NULL,
        [GeoRadius]           FLOAT             NULL,
        [IPDescription]       NVARCHAR(500)     NULL,
        [NewUserAttachmentId] UNIQUEIDENTIFIER  NULL,
        [ABN]                 NVARCHAR(11)      NULL,
        [CheckingMachineType] UNIQUEIDENTIFIER  NULL,
        [BlisterRobotTypeId]  NVARCHAR(MAX)     NULL,
        [IsMultiSite]         BIT               NOT NULL DEFAULT 0,
        [EnablePasswordAging] BIT               NULL,
        [PasswordAging]       INT               NULL,
        [ClusteredKey]        BIGINT            IDENTITY(1,1) NOT NULL,
        [RestrictPreferredBrand] BIT            NOT NULL DEFAULT 0,
        [HasThirdPartyPacking]   BIT            NULL,
        [PharmacyToInsert]       BIT            NULL,
        [YuyamaModelId]          NVARCHAR(MAX)  NULL,

        CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED ([Id])
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Warehouse_ClusteredKey] ON [dbo].[Warehouse] ([ClusteredKey]);
    CREATE NONCLUSTERED INDEX [IX_Warehouse_Name] ON [dbo].[Warehouse] ([Name]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WarehouseBankDetail')
BEGIN
    CREATE TABLE [dbo].[WarehouseBankDetail]
    (
        [Id]            UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [WarehouseId]   UNIQUEIDENTIFIER  NOT NULL,
        [BankName]      NVARCHAR(50)      NOT NULL,
        [BSB]           NVARCHAR(10)      NOT NULL,
        [AccountNumber] NVARCHAR(10)      NOT NULL,
        [ClusteredKey]  BIGINT            IDENTITY(1,1) NOT NULL,

        CONSTRAINT [PK_WarehouseBankDetail] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_WarehouseBankDetail_Warehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_WarehouseBankDetail_WarehouseId] ON [dbo].[WarehouseBankDetail] ([WarehouseId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WarehouseDocument')
BEGIN
    CREATE TABLE [dbo].[WarehouseDocument]
    (
        [Id]           UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [WarehouseId]  UNIQUEIDENTIFIER  NOT NULL,
        [DocType]      NVARCHAR(50)      NOT NULL,
        [Category]     NVARCHAR(50)      NOT NULL,
        [Name]         NVARCHAR(200)     NOT NULL,
        [DocContent]   VARBINARY(MAX)    NOT NULL,
        [ClusteredKey] BIGINT            IDENTITY(1,1) NOT NULL,

        CONSTRAINT [PK_WarehouseDocument] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_WarehouseDocument_Warehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_WarehouseDocument_WarehouseId] ON [dbo].[WarehouseDocument] ([WarehouseId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WarehouseHoliday')
BEGIN
    CREATE TABLE [dbo].[WarehouseHoliday]
    (
        [Id]           UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [WarehouseId]  UNIQUEIDENTIFIER  NOT NULL,
        [HolidayDate]  DATETIME2(7)      NOT NULL,
        [HolidayName]  NVARCHAR(500)     NULL,
        [Description]  NVARCHAR(2000)    NULL,
        [State]        NVARCHAR(10)      NOT NULL,
        [CreatedBy]    UNIQUEIDENTIFIER  NOT NULL,
        [CreatedDate]  DATETIME2(7)      NOT NULL,
        [UpdatedBy]    UNIQUEIDENTIFIER  NOT NULL,
        [UpdatedDate]  DATETIME2(7)      NOT NULL,
        [ClusteredKey] BIGINT            IDENTITY(1,1) NOT NULL,

        CONSTRAINT [PK_WarehouseHoliday] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_WarehouseHoliday_Warehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_WarehouseHoliday_WarehouseId] ON [dbo].[WarehouseHoliday] ([WarehouseId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WarehouseRobot')
BEGIN
    CREATE TABLE [dbo].[WarehouseRobot]
    (
        [Id]           UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [WarehouseId]  UNIQUEIDENTIFIER  NOT NULL,
        [Type]         NVARCHAR(10)      NOT NULL,
        [ClusteredKey] BIGINT            IDENTITY(1,1) NOT NULL,

        CONSTRAINT [PK_WarehouseRobot] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_WarehouseRobot_Warehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_WarehouseRobot_WarehouseId] ON [dbo].[WarehouseRobot] ([WarehouseId]);
END
GO
