using System.ComponentModel.DataAnnotations.Schema;

namespace Oswald_POS.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Change { get; set; }

        public string PaymentMethod { get; set; } = "Cash";

        public string ReceiptNumber { get; set; } = "";

        public bool IsVoided { get; set; } = false;

        public int? CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public List<SaleItem> SaleItems { get; set; } = new();
    }
}