-- ============================================================================
-- BestMed UserService — Event-sourced status stream
-- Database: sqldb-bmp-users-{env}
-- Run against: sqls-bmp-{env}.database.windows.net
-- Purpose: append-only event stream for the selected User status operation
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserStatusEvent')
BEGIN
	CREATE TABLE [dbo].[UserStatusEvent]
	(
		[EventId]     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
		[UserId]      UNIQUEIDENTIFIER NOT NULL,
		[Version]     INT              NOT NULL,
		[EventType]   NVARCHAR(50)     NOT NULL,
		[IsActive]    BIT              NULL,
		[Status]      NVARCHAR(20)     NULL,
		[OccurredAt]  DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),

		CONSTRAINT [PK_UserStatusEvent] PRIMARY KEY CLUSTERED ([EventId])
	);

	CREATE UNIQUE INDEX [UX_UserStatusEvent_UserId_Version]
		ON [dbo].[UserStatusEvent] ([UserId], [Version]);

	CREATE INDEX [IX_UserStatusEvent_UserId_OccurredAt]
		ON [dbo].[UserStatusEvent] ([UserId], [OccurredAt]);
END
GO