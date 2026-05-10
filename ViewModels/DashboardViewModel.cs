using Oswald_POS.Models;

namespace Oswald_POS.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodaySales { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalCustomers { get; set; }

        public decimal MondaySales { get; set; }
        public decimal TuesdaySales { get; set; }
        public decimal WednesdaySales { get; set; }
        public decimal ThursdaySales { get; set; }
        public decimal FridaySales { get; set; }
        public decimal SaturdaySales { get; set; }
        public decimal SundaySales { get; set; }

        public List<Sale> RecentSales { get; set; } = new();
        public List<TopProductViewModel> TopProducts { get; set; } = new();
        public List<Product> LowStockItems { get; set; } = new();
    }
}