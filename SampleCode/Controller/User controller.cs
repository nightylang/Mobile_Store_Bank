using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreBank.Data;

namespace MobileStoreBank.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => RedirectToAction("Login");

        public IActionResult Login() => View();

        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string identityToken, string passKey)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == identityToken && u.PasswordHash == passKey);

            if (user != null)
            {
                if (user.Role == "Admin") 
                    return RedirectToAction("Index", "Admin");
                
                return RedirectToAction("Index", "Bank");
            }

            ViewBag.ErrorMsg = "Security Validation Anomaly: Invalid authorization handshake.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            if (!ModelState.IsValid) return View(user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
    }
}
