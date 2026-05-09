using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oswald_POS.Data;
using Oswald_POS.Models;

namespace Oswald_POS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WorkersController : Controller
    {
        private readonly AppDbContext _context;

        public WorkersController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var workers = _context.Workers
                .OrderBy(w => w.FullName)
                .ToList();

            return View(workers);
        }

        [HttpPost]
        public IActionResult Create(Worker worker)
        {
            _context.Workers.Add(worker);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(Worker worker)
        {
            var existing = _context.Workers.Find(worker.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.FullName = worker.FullName;
            existing.Username = worker.Username;
            existing.Password = worker.Password;
            existing.Role = worker.Role;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var worker = _context.Workers.Find(id);

            if (worker != null && worker.Username != "admin")
            {
                _context.Workers.Remove(worker);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}