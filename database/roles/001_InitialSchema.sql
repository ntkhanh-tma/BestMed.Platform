-- ============================================================================
-- BestMed RoleService — Initial Schema
-- Database: sqldb-bmp-roles-{env}
-- Run against: sqls-bmp-{env}.database.windows.net
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserRole')
BEGIN
    CREATE TABLE [dbo].[UserRole]
    (
        [Id]              UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Role]            NVARCHAR(50)      NULL,
        [RoleName]        NVARCHAR(150)     NULL,
        [Description]     NVARCHAR(250)     NULL,
        [UserTypeId]      UNIQUEIDENTIFIER  NULL,
        [ClusteredKey]    BIGINT            IDENTITY(1,1) NOT NULL,
        [ConcurrencyStamp] NVARCHAR(MAX)   NULL,
        [NormalizedRole]  NVARCHAR(50)      NULL,

        CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id])
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_UserRole_ClusteredKey] ON [dbo].[UserRole] ([ClusteredKey]);
END
GO
