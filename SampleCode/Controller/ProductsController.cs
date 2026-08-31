using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileStoreBank.Data;

namespace MobileStoreBank.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();
            return View(products);
        }

        public IActionResult CRUD() => View();

        public async Task<IActionResult> Supply()
        {
            var inventory = await _context.Products.AsNoTracking().ToListAsync();
            return View(inventory);
        }

        public async Task<IActionResult> Category()
        {
            var categories = await _context.Products.AsNoTracking()
                .GroupBy(p => p.Category)
                .Select(g => new { CatName = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.CategoryData = categories;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(Product product)
        {
            if (!ModelState.IsValid) return View("CRUD", product);

            if (product.Id == 0)
                _context.Products.Add(product);
            else
                _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
