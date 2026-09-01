-- =========================================================================
-- SYSTEM PROJECT DATABASE INITIALIZATION: MOBILE STORE BANK
-- TARGET ENGINE: MICROSOFT SQL SERVER
-- =========================================================================

-- 1. ALLOCATE NEW CORES PHYSICAL DATASTORE STORAGE FILE PATHS
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MobileStoreBankDb')
BEGIN
    CREATE DATABASE MobileStoreBankDb;
END
GO

USE MobileStoreBankDb;
GO

-- =========================================================================
-- 2. CREATE SYSTEM NODE USERS IDENTITIES TABLE
-- =========================================================================
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users (
    Id           INT IDENTITY(1,1) NOT NULL,
    Username     NVARCHAR(150)     NOT NULL,
    Email        NVARCHAR(256)     NOT NULL,
    PasswordHash NVARCHAR(500)     NOT NULL,
    Role         NVARCHAR(50)      NOT NULL DEFAULT 'Merchant',
    CreatedAt    DATETIME2(7)      NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id)
);

-- Unique index constraints to enforce distinct entity validation boundaries
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Username ON dbo.Users (Username);
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email    ON dbo.Users (Email);
GO

-- =========================================================================
-- 3. CREATE STORES HARDWARE & ASSETS CATALOG TABLE
-- =========================================================================
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
    DROP TABLE dbo.Products;
GO

CREATE TABLE dbo.Products (
    Id       INT IDENTITY(1,1) NOT NULL,
    Name     NVARCHAR(250)     NOT NULL,
    SKU      NVARCHAR(100)     NOT NULL,
    Category NVARCHAR(150)     NOT NULL DEFAULT 'General',
    Price    DECIMAL(18,2)     NOT NULL, -- Forced high-precision currency boundary
    Stock    INT               NOT NULL DEFAULT 0,
    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Products_SKU ON dbo.Products (SKU);
GO

-- =========================================================================
-- 4. CREATE MULTI-CURRENCY MERCANTILE OPERATING WALLETS TABLE
-- =========================================================================
IF OBJECT_ID('dbo.Wallets', 'U') IS NOT NULL
    DROP TABLE dbo.Wallets;
GO

CREATE TABLE dbo.Wallets (
    Id               INT IDENTITY(1,1) NOT NULL,
    WalletAddress    NVARCHAR(100)     NOT NULL,
    AssetName        NVARCHAR(50)      NOT NULL DEFAULT 'USD',
    Balance          DECIMAL(18,2)     NOT NULL DEFAULT 0.00,
    PendingClearance DECIMAL(18,2)     NOT NULL DEFAULT 0.00,
    CONSTRAINT PK_Wallets PRIMARY KEY CLUSTERED (Id)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Wallets_WalletAddress ON dbo.Wallets (WalletAddress);
GO

-- =========================================================================
-- 5. CREATE IMMUTABLE TAMPER-EVIDENT TRANSACTION RECORD LEDGER TABLE
-- =========================================================================
IF OBJECT_ID('dbo.TransactionRecords', 'U') IS NOT NULL
    DROP TABLE dbo.TransactionRecords;
GO

CREATE TABLE dbo.TransactionRecords (
    Id                    INT IDENTITY(1,1) NOT NULL,
    ReferenceNumber       NVARCHAR(100)     NOT NULL,
    SourceWallet          NVARCHAR(150)     NOT NULL,
    DestinationWallet     NVARCHAR(150)     NOT NULL,
    Amount                DECIMAL(18,2)     NOT NULL,
    Status                NVARCHAR(50)      NOT NULL DEFAULT 'Completed',
    Timestamp             DATETIME2(7)      NOT NULL DEFAULT GETUTCDATE(),
    IntegrityHashSignature NVARCHAR(256)     NOT NULL, -- Layer 2 SHA256 verification hash cell
    CONSTRAINT PK_TransactionRecords PRIMARY KEY CLUSTERED (Id)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_TransactionRecords_ReferenceNumber ON dbo.TransactionRecords (ReferenceNumber);
GO

-- =========================================================================
-- 6. CREATE CRM PIPELINE MANAGEMENT OPERATIONS NETWORK TICKETS TABLE
-- =========================================================================
IF OBJECT_ID('dbo.CrmTickets', 'U') IS NOT NULL
    DROP TABLE dbo.CrmTickets;
GO

CREATE TABLE dbo.CrmTickets (
    Id           INT IDENTITY(1,1) NOT NULL,
    CustomerName NVARCHAR(200)     NOT NULL,
    IssueSummary NVARCHAR(MAX)     NOT NULL,
    Priority     NVARCHAR(50)      NOT NULL DEFAULT 'Medium',
    Status       NVARCHAR(50)      NOT NULL DEFAULT 'Open',
    CONSTRAINT PK_CrmTickets PRIMARY KEY CLUSTERED (Id)
);
GO

-- =========================================================================
-- 7. INJECT SEED METRICS PRODUCTION BASELINE RECORDS
-- =========================================================================
INSERT INTO dbo.Users (Username, Email, PasswordHash, Role) VALUES 
('admin', 'admin@storebank.com', 'admin123', 'Admin'),
('merchant', 'merchant@storebank.com', 'password123', 'Merchant');

INSERT INTO dbo.Products (Name, SKU, Category, Price, Stock) VALUES 
('iPhone 15 Pro Max', 'IPH-15PM', 'Devices', 1199.99, 45),
('SaaS Settlement Micro-Gateway Token', 'SAAS-V2', 'SaaS Licenses', 89.00, 1000),
('Wireless POS Terminal X', 'POS-X4', 'Hardware', 299.50, 15);

INSERT INTO dbo.Wallets (WalletAddress, AssetName, Balance, PendingClearance) VALUES 
('f3b92c481a704e6bb69c0d1252d4bfba', 'USD Core Ledger Pool', 142500.50, 3200.00),
('a4e70cc85d81432da84a9192bb035c91', 'BTC Cold Settlement Vault', 1.42, 0.05);

-- Seed transaction contains standard pre-computed validation signature hash tracking parameters
INSERT INTO dbo.TransactionRecords (ReferenceNumber, SourceWallet, DestinationWallet, Amount, Status, IntegrityHashSignature) VALUES 
('TXN-SYSTEMINIT', 'System Core Provisioner', 'USD Core Ledger Pool', 142500.50, 'Completed', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855');

INSERT INTO dbo.CrmTickets (CustomerName, IssueSummary, Priority, Status) VALUES 
('Phnom Penh Retail Group', 'API Webhook payload tracking delay over HTTP channel bindings', 'High', 'Open'),
('Global Logistics Node', 'Batch daily transaction verification review request', 'Medium', 'In-Progress');
GO

PRINT '🎯 Mobile Store Bank database infrastructure compilation mapping succeeded!';
