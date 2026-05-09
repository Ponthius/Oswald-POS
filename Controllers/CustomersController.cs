using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oswald_POS.Data;
using Oswald_POS.Models;

namespace Oswald_POS.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var customers = _context.Customers
                .OrderBy(c => c.FullName)
                .ToList();

            return View(customers);
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(Customer customer)
        {
            var existing = _context.Customers.Find(customer.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.FullName = customer.FullName;
            existing.Phone = customer.Phone;
            existing.CreditBalance = customer.CreditBalance;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}