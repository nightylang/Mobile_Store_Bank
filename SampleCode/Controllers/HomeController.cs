using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StoreMobile.Models;

namespace StoreMobile.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() { 
            return View();
        }
        
        public IActionResult About() { 
            return View();
        }
        
        public IActionResult Product() {
            RedirectToAction("Index", "Products");
        }        
        public IActionResult Bank() { 
            RedirectToAction("Index", "Bank");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
