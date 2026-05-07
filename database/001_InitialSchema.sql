-- BestMed UserService Database Schema
-- Run this against each environment's Azure SQL Database

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [ExternalId]  NVARCHAR(256)    NOT NULL,
        [Email]       NVARCHAR(256)    NOT NULL,
        [FirstName]   NVARCHAR(128)    NULL,
        [LastName]    NVARCHAR(128)    NULL,
        [PhoneNumber] NVARCHAR(64)     NULL,
        [IsActive]    BIT              NOT NULL DEFAULT 1,
        [CreatedAt]   DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt]   DATETIME2(7)     NULL,

        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Users_ExternalId] UNIQUE ([ExternalId]),
        CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
    );

    CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
    CREATE NONCLUSTERED INDEX [IX_Users_IsActive] ON [dbo].[Users] ([IsActive]) INCLUDE ([Email], [FirstName], [LastName]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserAddresses')
BEGIN
    CREATE TABLE [dbo].[UserAddresses]
    (
        [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [UserId]    UNIQUEIDENTIFIER NOT NULL,
        [Street]    NVARCHAR(256)    NOT NULL,
        [City]      NVARCHAR(128)    NULL,
        [State]     NVARCHAR(64)     NULL,
        [ZipCode]   NVARCHAR(32)     NULL,
        [Country]   NVARCHAR(64)     NULL,
        [IsPrimary] BIT              NOT NULL DEFAULT 0,

        CONSTRAINT [PK_UserAddresses] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_UserAddresses_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_UserAddresses_UserId] ON [dbo].[UserAddresses] ([UserId]);
END
GO
