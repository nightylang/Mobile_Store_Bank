using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMobile.Data;

namespace StoreMobile.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalLiquidity = await _context.Wallets.AsNoTracking().SumAsync(w => w.Balance);
            ViewBag.ActiveUsersCount = await _context.Users.AsNoTracking().CountAsync();
            ViewBag.ActiveTickets = await _context.CrmTickets.AsNoTracking().CountAsync(t => t.Status != "Closed");
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> ProdList()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> CRM()
        {
            var tickets = await _context.CrmTickets.AsNoTracking().ToListAsync();
            return View(tickets);
        }
    }
}
