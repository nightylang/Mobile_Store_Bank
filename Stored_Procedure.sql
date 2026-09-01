USE MobileStoreBankDb;
GO

IF OBJECT_ID('dbo.sp_ExecuteStoreSettlement', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ExecuteStoreSettlement;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
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
    -- 1. ENFORCE INLINE ERROR TERMINATION METRICS
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- 2. ASYMMETRIC API HARDWARE SECURITY TOKEN COMPLIANCE GATE
    IF (@SecurityToken IS NULL OR @SecurityToken <> 'POS-SECURE-KEY-HASH-V2')
    BEGIN
        RAISERROR('Security Handshake Refused: Unauthorized token verification key mapped over cleartext pipe.', 16, 1);
        RETURN;
    END;

    -- 3. ATTRIBUTE STATE CONSTRAINT VALIDATION
    IF (@SettlementAmount <= 0.00)
    BEGIN
        RAISERROR('Operational Anomaly: Settlement transaction amount threshold must be greater than zero.', 16, 2);
        RETURN;
    END;

    -- 4. INITIALIZE SERIALIZABLE ISOLATION LEVEL TRANSACTION BLOCK
    -- Serializable isolation forces locks across rows to completely eliminate phantom read data mutations.
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Check if target wallet exists and lock the row exclusively for this execution pipeline thread
        IF NOT EXISTS (SELECT 1 FROM dbo.Wallets WITH (XLOCK, ROWLOCK) WHERE AssetName = @TargetAssetPool)
        BEGIN
            RAISERROR('Configuration Exception: Mapped settlement asset pool target does not exist inside core system indices.', 16, 3);
        END;

        -- Update the core settlement asset balance pool
        UPDATE dbo.Wallets
        SET Balance = Balance + @SettlementAmount
        WHERE AssetName = @TargetAssetPool;

        -- Extract the runtime calculated tracking balance cell parameters for the output hook variables
        SELECT @NewBalanceOutput = Balance
        FROM dbo.Wallets
        WHERE AssetName = @TargetAssetPool;

        -- Write the immutable record directly onto the physical database ledger rows
        INSERT INTO dbo.TransactionRecords (
            ReferenceNumber, 
            SourceWallet, 
            DestinationWallet, 
            Amount, 
            Status, 
            Timestamp, 
            IntegrityHashSignature
        ) VALUES (
            @GeneratedTxnRef,
            N'POS-Terminal-' + @TerminalId,
            @TargetAssetPool,
            @SettlementAmount,
            N'Completed',
            GETUTCDATE(),
            @IntegrityHash
        );

        -- If all atomic operations complete successfully, commit changes safely to the datastore
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Check if transaction state requires explicit system state rolling regression loops
        IF (XACT_STATE()) <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        -- Bubble the error message text upward to the ASP.NET Core calling pipeline
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

PRINT '🎯 Transactional Settlement Stored Procedure deployed to Microsoft SQL Server successfully!';
