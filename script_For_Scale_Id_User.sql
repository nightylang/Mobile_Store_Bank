USE MobileStoreBankDb;
GO

-- 1. Drop existing index bindings to safely permit identity data modifications
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username')
    DROP INDEX IX_Users_Username ON dbo.Users;
GO

-- 2. Alter the Primary Key field configuration to BIGINT
-- NOTE: In production environments containing data, verify truncation rules before running ALTER statements
ALTER TABLE dbo.Users 
    ALTER COLUMN Id BIGINT NOT NULL;
GO

-- Re-establish the baseline primary key constraint array mechanics
ALTER TABLE dbo.Users
    DROP CONSTRAINT PK_Users;
GO

ALTER TABLE dbo.Users
    ADD CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id);
GO

-- Re-map secondary unique non-clustered validation trackers
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Username ON dbo.Users (Username);
GO

PRINT '🎯 SQL Server User Primary Key architecture scaled to BIGINT successfully!';
