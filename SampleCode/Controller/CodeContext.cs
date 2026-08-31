using Microsoft.AspNetCore.Mvc;

namespace MobileStoreBank.Controllers
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
    }
}
