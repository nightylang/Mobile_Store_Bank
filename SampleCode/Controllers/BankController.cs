using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMobile.Data;

namespace StoreMobile.Controllers
{
    public class BankController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BankController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var wallets = await _context.Wallets.AsNoTracking().ToListAsync();
            return View(wallets);
        }

        public async Task<IActionResult> Wallet()
        {
            var wallets = await _context.Wallets.AsNoTracking().ToListAsync();
            return View(wallets);
        }

        public async Task<IActionResult> History()
        {
            var records = await _context.TransactionRecords.AsNoTracking().ToListAsync();
            return View(records);
        }

        public async Task<IActionResult> Account()
        {
            ViewBag.ProfileUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Role == "Merchant");
            return View();
        }
    }
}
