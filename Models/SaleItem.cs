using System.ComponentModel.DataAnnotations.Schema;

namespace Oswald_POS.Models
{
    public class SaleItem
    {
        public int Id { get; set; }

        public int SaleId { get; set; }

        public Sale? Sale { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}