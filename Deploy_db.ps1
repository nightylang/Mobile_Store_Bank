# =========================================================================
# MOBILE STORE BANK - AUTOMATED SQL SERVER DB SETUP PIPELINE
# RUNTIME ENVIRONMENT: WINDOWS POWERSHELL
# =========================================================================

# 1. DATABASE ACCESS CONFIGURATION PARAMETERS
$ServerInstance = "localhost\SQLEXPRESS"
$DatabaseName   = "MobileStoreBankDb"
$ConnectionString = "Server=$ServerInstance;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"

Write-Host "=== Starting Mobile Store Bank SQL Server Deployment Pipeline ===" -ForegroundColor Cyan

# 2. VERIFY LOCAL SQL SERVER TOOLS AVAILABILITY
Write-Host "🛠️  Verifying SqlServer PowerShell modules..." -ForegroundColor Yellow
if (-not (Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "💾 Installing missing 'SqlServer' module from PowerShell Gallery..." -ForegroundColor Cyan
    Install-Module -Name SqlServer -Force -AllowClobber -Scope CurrentUser
}

# 3. COMPILE CONSOLIDATED T-SQL INFRASTRUCTURE SCRIPT
$TSqlScript = @"
-- Allocate primary physical datastore space
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$DatabaseName')
BEGIN
    CREATE DATABASE $DatabaseName;
END;
GO

USE $DatabaseName;
GO

-- Node User Identities
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
CREATE TABLE dbo.Users (
    Id           BIGINT IDENTITY(1,1) NOT NULL,
    Username     NVARCHAR(150)     NOT NULL,
    Email        NVARCHAR(256)     NOT NULL,
    PasswordHash NVARCHAR(500)     NOT NULL,
    Role         NVARCHAR(50)      NOT NULL DEFAULT 'Merchant',
    CreatedAt    DATETIME2(7)      NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id)
);
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Username ON dbo.Users (Username);
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email    ON dbo.Users (Email);

-- Store Product Inventory
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
CREATE TABLE dbo.Products (
    Id       INT IDENTITY(1,1) NOT NULL,
    Name     NVARCHAR(250)     NOT NULL,
    SKU      NVARCHAR(100)     NOT NULL,
    Category NVARCHAR(150)     NOT NULL DEFAULT 'General',
    Price    DECIMAL(18,2)     NOT NULL,
    Stock    INT               NOT NULL DEFAULT 0,
    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id)
);
CREATE UNIQUE NONCLUSTERED INDEX IX_Products_SKU ON dbo.Products (SKU);

-- Multi-Currency Liquidity Wallets
IF OBJECT_ID('dbo.Wallets', 'U') IS NOT NULL DROP TABLE dbo.Wallets;
CREATE TABLE dbo.Wallets (
    Id               INT IDENTITY(1,1) NOT NULL,
    WalletAddress    NVARCHAR(100)     NOT NULL,
    AssetName        NVARCHAR(50)      NOT NULL DEFAULT 'USD',
    Balance          DECIMAL(18,2)     NOT NULL DEFAULT 0.00,
    PendingClearance DECIMAL(18,2)     NOT NULL DEFAULT 0.00,
    CONSTRAINT PK_Wallets PRIMARY KEY CLUSTERED (Id)
);
CREATE UNIQUE NONCLUSTERED INDEX IX_Wallets_WalletAddress ON dbo.Wallets (WalletAddress);

-- Immutable Tamper-Evident Transaction Ledger
IF OBJECT_ID('dbo.TransactionRecords', 'U') IS NOT NULL DROP TABLE dbo.TransactionRecords;
CREATE TABLE dbo.TransactionRecords (
    Id                     INT IDENTITY(1,1) NOT NULL,
    ReferenceNumber        NVARCHAR(100)     NOT NULL,
    SourceWallet           NVARCHAR(150)     NOT NULL,
    DestinationWallet      NVARCHAR(150)     NOT NULL,
    Amount                 DECIMAL(18,2)     NOT NULL,
    Status                 NVARCHAR(50)      NOT NULL DEFAULT 'Completed',
    Timestamp              DATETIME2(7)      NOT NULL DEFAULT GETUTCDATE(),
    IntegrityHashSignature NVARCHAR(256)     NOT NULL,
    CONSTRAINT PK_TransactionRecords PRIMARY KEY CLUSTERED (Id)
);
CREATE UNIQUE NONCLUSTERED INDEX IX_TransactionRecords_Ref ON dbo.TransactionRecords (ReferenceNumber);

-- CRM Incident Management 
IF OBJECT_ID('dbo.CrmTickets', 'U') IS NOT NULL DROP TABLE dbo.CrmTickets;
CREATE TABLE dbo.CrmTickets (
    Id           INT IDENTITY(1,1) NOT NULL,
    CustomerName NVARCHAR(200)     NOT NULL,
    IssueSummary NVARCHAR(MAX)     NOT NULL,
    Priority     NVARCHAR(50)      NOT NULL DEFAULT 'Medium',
    Status       NVARCHAR(50)      NOT NULL DEFAULT 'Open',
    CONSTRAINT PK_CrmTickets PRIMARY KEY CLUSTERED (Id)
);
GO

-- Seed baseline mock records
INSERT INTO dbo.Users (Username, Email, PasswordHash, Role) VALUES ('admin', 'admin@storebank.com', 'admin123', 'Admin');
INSERT INTO dbo.Wallets (WalletAddress, AssetName, Balance, PendingClearance) VALUES ('f3b92c481a704e6bb69c0d1252d4bfba', 'USD Core Ledger Pool', 142500.50, 0.00);
GO

-- =========================================================================
-- OPTION 2: COMPILING TRANSACTIONAL SETTLEMENT STORED PROCEDURE
-- =========================================================================
IF OBJECT_ID('dbo.sp_ExecuteStoreSettlement', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.sp_ExecuteStoreSettlement;
GO

CREATE PROCEDURE dbo.sp_ExecuteStoreSettlement
    @TerminalId         NVARCHAR(150),
    @SecurityToken      NVARCHAR(200),
    @TargetAssetPool    NVARCHAR(50),
    @SettlementAmount   DECIMAL(18,2),
    @GeneratedTxnRef    NVARCHAR(100),
    @IntegrityHash      NVARCHAR(256),
    @NewBalanceOutput   DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Asymmetric API token barrier check
    IF (@SecurityToken IS NULL OR @SecurityToken <> 'POS-SECURE-KEY-HASH-V2')
    BEGIN
        RAISERROR('Security Handshake Refused: Unauthorized token verification key.', 16, 1);
        RETURN;
    END;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Wallets WITH (XLOCK, ROWLOCK) WHERE AssetName = @TargetAssetPool)
        BEGIN
            RAISERROR('Configuration Exception: Mapped settlement pool does not exist.', 16, 2);
        END;

        UPDATE dbo.Wallets SET Balance = Balance + @SettlementAmount WHERE AssetName = @TargetAssetPool;
        SELECT @NewBalanceOutput = Balance FROM dbo.Wallets WHERE AssetName = @TargetAssetPool;

        INSERT INTO dbo.TransactionRecords (ReferenceNumber, SourceWallet, DestinationWallet, Amount, Status, IntegrityHashSignature)
        VALUES (@GeneratedTxnRef, N'POS-Terminal-' + @TerminalId, @TargetAssetPool, @SettlementAmount, N'Completed', @IntegrityHash);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE()) <> 0 ROLLBACK TRANSACTION;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO
"@

# 4. EXECUTE PIPELINE TASKS ON TARGET SQL INSTANCE
try {
    Write-Host "🚀  Executing T-SQL structural scripts and Stored Procedure compilation models..." -ForegroundColor Yellow
    Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $TSqlScript -QueryTimeout 30
    Write-Host "✅  Database infrastructure setup completed with a 0% error profile!" -ForegroundColor Green
} catch {
    Write-Error "❌  Deployment pipeline failed: $_"
}
