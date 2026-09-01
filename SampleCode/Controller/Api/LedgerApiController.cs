using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MobileStoreBank.Data;

namespace MobileStoreBank.Controllers.Api
{
    [ApiController]
    [Route("api/ledger")]
    public class LedgerApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LedgerApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("settle-sp")]
        public async Task<IActionResult> ExecuteSettleViaStoredProcedure(
            [FromHeader(Name = "X-POS-Terminal-ID")] string terminalId,
            [FromHeader(Name = "X-POS-Security-Token")] string securityToken,
            [FromBody] PosSettlementRequest payload)
        {
            // 1. Generate runtime metadata values
            string txnRef = $"POS-TXN-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
            
            // Build temporary mock model instance to pass down to your HMAC protection service layer
            var mockTxn = new Models.TransactionRecord {
                ReferenceNumber = txnRef, SourceWallet = $"POS-Terminal-{terminalId}",
                DestinationWallet = payload.TargetAssetPool, Amount = payload.Amount, Status = "Completed"
            };
            string integrityHash = LedgerGuardService.ComputeRecordSignature(mockTxn);

            // 2. Map strict strongly-typed SQL parameters matching our stored procedure signature
            var paramTerminal  = new SqlParameter("@TerminalId", SqlDbType.NVarChar, 150) { Value = terminalId ?? (object)DBNull.Value };
            var paramToken     = new SqlParameter("@SecurityToken", SqlDbType.NVarChar, 200) { Value = securityToken ?? (object)DBNull.Value };
            var paramPool      = new SqlParameter("@TargetAssetPool", SqlDbType.NVarChar, 50) { Value = payload.TargetAssetPool };
            var paramAmount    = new SqlParameter("@SettlementAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = payload.Amount };
            var paramRef       = new SqlParameter("@GeneratedTxnRef", SqlDbType.NVarChar, 100) { Value = txnRef };
            var paramHash      = new SqlParameter("@IntegrityHash", SqlDbType.NVarChar, 256) { Value = integrityHash };
            
            // Output parameter target to intercept calculated asset values from the SQL Server engine
            var paramOutBal    = new SqlParameter("@NewInternalBalance", SqlDbType.Decimal) {
                Precision = 18, Scale = 2, Direction = ParameterDirection.Output
            };

            try
            {
                // Execute database context procedure natively
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_ExecuteStoreSettlement @TerminalId, @SecurityToken, @TargetAssetPool, @SettlementAmount, @GeneratedTxnRef, @IntegrityHash, @NewInternalBalance OUTPUT",
                    paramTerminal, paramToken, paramPool, paramAmount, paramRef, paramHash, paramOutBal);

                decimal updatedBalance = (decimal)paramOutBal.Value;

                return Ok(new
                {
                    Status = "Verified",
                    TransactionRef = txnRef,
                    NewInternalBalance = updatedBalance,
                    SystemChannel = "Stored Procedure Matrix Optimization Layer"
                });
            }
            catch (SqlException ex)
            {
                // Intercept and handle SQL Server bubble error indicators safely
                return StatusCode(400, new { Error = $"Database Rule Rejection: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Pipeline Internal Crash Anomaly: {ex.Message}" });
            }
        }
    }
}
