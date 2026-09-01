using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreBank.Data;
using MobileStoreBank.Models;

namespace MobileStoreBank.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. PUBLIC WEB USER INTERFACES MATRIX MAPPINGS
        
        public async Task<IActionResult> Index()
        {
            // Pull the latest 15 attendance logging entries asynchronously for display rows
            var logs = await _context.AttendanceRecords
                .AsNoTracking()
                .OrderByDescending(a => a.Timestamp)
                .Take(15)
                .ToListAsync();
            return View(logs);
        }

        public IActionResult Scan() => View();

        public IActionResult CheckIn() => View();

        public IActionResult CheckOut() => View();


        // 2. UNENCRYPTED RECTTEXT REST API INTEGRATION LANE FOR PYTHON ROBOT NODES
        // Endpoint Target: POST http://localhost:5000/attendance/process-scan
        
        [HttpPost]
        [Route("attendance/process-scan")
        public async Task<IActionResult> ProcessScan([FromBody] PythonScanPayload payload)
        {
            if (payload == null || string.IsNullOrWhiteSpace(payload.UsernameToken))
            {
                return BadRequest(new { Error = "Invalid automation packet payload." });
            }

            // Verify if the scanned username exists within your scaled 1-billion SQL records pool
            var matchedUser = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == payload.UsernameToken);

            if (matchedUser == null)
            {
                return NotFound(new { Error = "Identity Validation Error: Scanned badge token not recognized." });
            }

            // Enforce clean, predictable logic: check if the node's previous record was a check-in
            var latestLog = await _context.AttendanceRecords
                .Where(a => a.UserIdentity == matchedUser.Username)
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefaultAsync();

            string determinedAction = (latestLog == null || latestLog.ActionType == "CheckOut") ? "CheckIn" : "CheckOut";

            // Commit atomic log entity safely to MS SQL Server
            var trackingLog = new AttendanceRecord
            {
                UserIdentity = matchedUser.Username,
                ActionType = determinedAction,
                Timestamp = DateTime.UtcNow,
                TerminalNode = payload.NodeIdentifier ?? "PYTHON-VISION-NODE"
            };

            _context.AttendanceRecords.Add(trackingLog);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Status = "Success",
                User = matchedUser.Username,
                Action = determinedAction,
                LoggedTime = trackingLog.Timestamp.ToString("yyyy-MM-dd HH:mm:ss") + " UTC"
            });
        }
    }

    public class PythonScanPayload
    {
        public string UsernameToken { get; set; } = string.Empty;
        public string? NodeIdentifier { get; set; }
    }
}
