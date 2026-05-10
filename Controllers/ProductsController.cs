using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oswald_POS.Data;
using Oswald_POS.Models;

namespace Oswald_POS.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();

            var products = _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToList();

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (imageFile != null)
            {
                product.ImageUrl = await SaveImage(imageFile);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile? imageFile)
        {
            var existing = _context.Products.Find(product.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = product.Name;
            existing.Barcode = product.Barcode;
            existing.Price = product.Price;
            existing.StockQuantity = product.StockQuantity;
            existing.LowStockLevel = product.LowStockLevel;
            existing.CategoryId = product.CategoryId;

            if (imageFile != null)
            {
                existing.ImageUrl = await SaveImage(imageFile);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AdjustStock(int id, int adjustment)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            product.StockQuantity += adjustment;

            if (product.StockQuantity < 0)
            {
                product.StockQuantity = 0;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool productHasSales = _context.SaleItems.Any(s => s.ProductId == id);

            if (productHasSales)
            {
                TempData["Error"] = "This product has sales history, so it cannot be deleted. Set stock to 0 instead.";
                return RedirectToAction("Index");
            }

            var product = _context.Products.Find(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            string uploadFolder = Path.Combine(_environment.WebRootPath, "images", "products");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return "/images/products/" + fileName;
        }
    }
}