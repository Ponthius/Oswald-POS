using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oswald_POS.Data;
using Oswald_POS.Models;
using Oswald_POS.ViewModels;

namespace Oswald_POS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);

            if (today.DayOfWeek == DayOfWeek.Sunday)
            {
                startOfWeek = today.AddDays(-6);
            }

            var endOfWeek = startOfWeek.AddDays(7);

            var weeklySales = _context.Sales
                .Where(s => !s.IsVoided && s.SaleDate >= startOfWeek && s.SaleDate < endOfWeek)
                .ToList();

            var saleItems = _context.SaleItems
                .Include(si => si.Product)
                .Include(si => si.Sale)
                .Where(si => si.Sale != null && !si.Sale.IsVoided)
                .ToList();

            var topProductIds = saleItems
                .GroupBy(si => si.ProductId)
                .OrderByDescending(g => g.Sum(x => x.Quantity))
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            var topProducts = _context.Products
                .Where(p => topProductIds.Contains(p.Id))
                .ToList();

            var model = new DashboardViewModel
            {
                TodaySales = _context.Sales
                    .Where(s => !s.IsVoided && s.SaleDate >= today && s.SaleDate < tomorrow)
                    .Sum(s => (decimal?)s.TotalAmount) ?? 0,

                TotalProducts = _context.Products.Count(),

                LowStockProducts = _context.Products
                    .Count(p => p.StockQuantity <= p.LowStockLevel),

                TotalCustomers = _context.Customers.Count(),

                MondaySales = weeklySales
                    .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Monday)
                    .Sum(s => s.TotalAmount),

                TuesdaySales = weeklySales
                    .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Tuesday)
                    .Sum(s => s.TotalAmount),

                WednesdaySales = weeklySales
                    .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Wednesday)
                    .Sum(s => s.TotalAmount),

                ThursdaySales = weeklySales
                    .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Thursday)
                    .Sum(s => s.TotalAmount),

                FridaySales = weeklySales
                    .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Friday)
                    .Sum(s => s.TotalAmount),

                SaturdaySales = weeklySales
                    .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Saturday)
                    .Sum(s => s.TotalAmount),

                SundaySales = weeklySales
                    .Where(s => s.SaleDate.DayOfWeek == DayOfWeek.Sunday)
                    .Sum(s => s.TotalAmount),

                RecentSales = _context.Sales
                    .Include(s => s.Customer)
                    .Where(s => !s.IsVoided)
                    .OrderByDescending(s => s.SaleDate)
                    .Take(6)
                    .ToList(),

                TopProducts = topProducts
            };

            return View(model);
        }
    }
}