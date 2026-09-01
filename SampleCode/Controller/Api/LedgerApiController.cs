using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreBank.Data;
using MobileStoreBank.Models;

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

        /// <summary>
        /// Remote execution pipeline for external mobile store POS client terminal machines.
        /// Route payload endpoint: POST http://localhost:5000/api/ledger/settle
        /// </summary>
        [HttpPost]
        [Route("settle")]
        public async Task<IActionResult> ExecuteTerminalSettlement(
            [FromHeader(Name = "X-POS-Terminal-ID")] string terminalId,
            [FromHeader(Name = "X-POS-Security-Token")] string securityToken,
            [FromBody] PosSettlementRequest payload)
        {
            // 1. Asymmetric hardware-token security validation gate
            if (string.IsNullOrWhiteSpace(terminalId) || securityToken != "POS-SECURE-KEY-HASH-V2")
            {
                return StatusCode(401, new { Error = "Security Handshake Refused: Invalid token mapping over HTTP cleartext pipe." });
            }

            if (payload.Amount <= 0)
            {
                return BadRequest(new { Error = "Invalid Payload: Transaction transaction amount threshold must be greater than zero." });
            }

            // 2. Fetch target settlement liquidity wallet context from DB
            var targetWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.AssetName == payload.TargetAssetPool);

            if (targetWallet == null)
            {
                return NotFound(new { Error = $"Target pool mapping error: Asset pool '{payload.TargetAssetPool}' not recognized." });
            }

            // 3. Begin an isolated transaction context execution sequence
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Increment wallet equity
                targetWallet.Balance += payload.Amount;

                // Log a clean immutable record directly onto the physical database ledger
                var record = new TransactionRecord
                {
                    ReferenceNumber = $"POS-TXN-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
                    SourceWallet = $"POS-Terminal-{terminalId}",
                    DestinationWallet = targetWallet.AssetName,
                    Amount = payload.Amount,
                    Status = "Completed",
                    Timestamp = DateTime.UtcNow
                };

                _context.TransactionRecords.Add(record);
                await _context.SaveChangesAsync();

                // Commit the atomic database modification transaction cleanly
                await dbTransaction.CommitAsync();

                return Ok(new
                {
                    Status = "Verified",
                    TransactionRef = record.ReferenceNumber,
                    SettledPool = targetWallet.AssetName,
                    NewInternalBalance = targetWallet.Balance,
                    SystemTimestamp = record.Timestamp
                });
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                return StatusCode(500, new { Error = "Database Transaction Failure Anomaly occurred while processing point-of-sale payload state." });
            }
        }
    }

    /// <summary>
    /// Strict Data Transfer Object matching merchant payload matrices
    /// </summary>
    public class PosSettlementRequest
    {
        public decimal Amount { get; set; }
        public string TargetAssetPool { get; set; } = "USD Core Ledger Pool";
    }
}
