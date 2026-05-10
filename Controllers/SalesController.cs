using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oswald_POS.Data;
using Oswald_POS.Models;
using Oswald_POS.ViewModels;
using System.Security.Claims;
using System.Text;

namespace Oswald_POS.Controllers
{
    [Authorize(Roles = "Admin,Manager,Cashier")]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Customers = _context.Customers.ToList();

            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .ToList();

            return View(products);
        }

        [HttpPost]
        public IActionResult CompleteSale([FromBody] SaleRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { message = "Cart is empty." });
            }

            var workerIdClaim = User.FindFirst("WorkerId")?.Value;
            int? workerId = null;

            if (int.TryParse(workerIdClaim, out int parsedWorkerId))
            {
                workerId = parsedWorkerId;
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                decimal total = 0;

                var sale = new Sale
                {
                    SaleDate = DateTime.Now,
                    PaymentMethod = request.PaymentMethod,
                    AmountPaid = request.AmountPaid,
                    ReceiptNumber = "OSW-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CustomerId = request.CustomerId,
                    WorkerId = workerId
                };

                foreach (var item in request.Items)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);

                    if (product == null)
                    {
                        return BadRequest(new { message = "Product not found." });
                    }

                    if (item.Quantity <= 0)
                    {
                        return BadRequest(new { message = "Invalid quantity." });
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        return BadRequest(new { message = $"{product.Name} has only {product.StockQuantity} left in stock." });
                    }

                    decimal itemTotal = product.Price * item.Quantity;
                    total += itemTotal;

                    sale.SaleItems.Add(new SaleItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = itemTotal
                    });

                    product.StockQuantity -= item.Quantity;
                }

                sale.TotalAmount = total;
                sale.Change = request.AmountPaid > total ? request.AmountPaid - total : 0;

                if (request.CustomerId != null && request.AmountPaid < total)
                {
                    var customer = _context.Customers.FirstOrDefault(c => c.Id == request.CustomerId);

                    if (customer != null)
                    {
                        customer.CreditBalance += total - request.AmountPaid;
                    }
                }

                _context.Sales.Add(sale);
                _context.SaveChanges();

                transaction.Commit();

                return Ok(new
                {
                    message = "Sale completed successfully.",
                    saleId = sale.Id,
                    receiptUrl = Url.Action("Receipt", "Sales", new { id = sale.Id })
                });
            }
            catch
            {
                transaction.Rollback();
                return BadRequest(new { message = "Sale failed. Try again." });
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult History()
        {
            var sales = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Worker)
                .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToList();

            return View(sales);
        }

        public IActionResult Receipt(int id)
        {
            var sale = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Worker)
                .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        public IActionResult DownloadReceipt(int id)
        {
            var sale = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Worker)
                .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            var receipt = new StringBuilder();

            receipt.AppendLine("OSWALD POS");
            receipt.AppendLine("Tel: 0700 000 000");
            receipt.AppendLine("Location: Kampala, Uganda");
            receipt.AppendLine("Mini Receipt");
            receipt.AppendLine("-----------------------------");
            receipt.AppendLine($"Receipt: {sale.ReceiptNumber}");
            receipt.AppendLine($"Date: {sale.SaleDate}");
            receipt.AppendLine($"Customer: {sale.Customer?.FullName ?? "Walk-in Customer"}");
            receipt.AppendLine($"Cashier: {sale.Worker?.FullName ?? "Unknown"}");
            receipt.AppendLine("-----------------------------");

            foreach (var item in sale.SaleItems)
            {
                receipt.AppendLine($"{item.Product?.Name} x {item.Quantity}");
                receipt.AppendLine($"UGX {item.UnitPrice:N0} = UGX {item.TotalPrice:N0}");
            }

            receipt.AppendLine("-----------------------------");
            receipt.AppendLine($"Total: UGX {sale.TotalAmount:N0}");
            receipt.AppendLine($"Paid: UGX {sale.AmountPaid:N0}");
            receipt.AppendLine($"Change: UGX {sale.Change:N0}");
            receipt.AppendLine($"Payment: {sale.PaymentMethod}");
            receipt.AppendLine("-----------------------------");
            receipt.AppendLine("Thank you for shopping with us.");

            byte[] bytes = Encoding.UTF8.GetBytes(receipt.ToString());

            return File(bytes, "text/plain", $"{sale.ReceiptNumber}.txt");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Void(int id)
        {
            var sale = _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefault(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            if (!sale.IsVoided)
            {
                foreach (var item in sale.SaleItems)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);

                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                }

                sale.IsVoided = true;
                _context.SaveChanges();
            }

            return RedirectToAction("History");
        }
    }
}