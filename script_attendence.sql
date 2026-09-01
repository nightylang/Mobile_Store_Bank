USE MobileStoreBankDb;
GO

IF OBJECT_ID('dbo.AttendanceRecords', 'U') IS NOT NULL
    DROP TABLE dbo.AttendanceRecords;
GO

CREATE TABLE dbo.AttendanceRecords (
    Id            BIGINT IDENTITY(1,1) NOT NULL, -- Scaled for 64-bit high-capacity indices
    UserIdentity  NVARCHAR(150)     NOT NULL,
    Timestamp     DATETIME2(7)      NOT NULL DEFAULT GETUTCDATE(),
    ActionType    NVARCHAR(50)      NOT NULL, -- 'CheckIn' or 'CheckOut'
    TerminalNode  NVARCHAR(100)     NOT NULL DEFAULT 'PYTHON-VISION-NODE',
    CONSTRAINT PK_AttendanceRecords PRIMARY KEY CLUSTERED (Id)
);

CREATE NONCLUSTERED INDEX IX_AttendanceRecords_User ON dbo.AttendanceRecords (UserIdentity);
GO
