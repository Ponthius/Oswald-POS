using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oswald_POS.Data;
using Oswald_POS.Models;

namespace Oswald_POS.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}