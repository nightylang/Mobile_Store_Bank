using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMobile.Data;

namespace StoreMobile.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Controllers/UserController.cs modification snippet
        public async Task<IActionResult> Index()
        {
            // Queries the baseline default merchant profile from your SQLite data table context
            var activeProfile = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Role == "Merchant");

            if (activeProfile == null)
            {
                return RedirectToAction("Login");
            }

            return View(activeProfile);
        }

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
        // Controllers/UserController.cs parameter optimization pattern snippet
[HttpGet]
public async Task<IActionResult> InspectNodeDetails(long id) // Explicit long parsing input parameter
{
    var targetNode = await _context.Users.AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id);

    if (targetNode == null) return NotFound();

    return View("Index", targetNode);
}

    }
}
