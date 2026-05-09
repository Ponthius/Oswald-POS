using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oswald_POS.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Barcode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockLevel { get; set; } = 5;

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}