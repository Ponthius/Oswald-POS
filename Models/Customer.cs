using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oswald_POS.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public string? Phone { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditBalance { get; set; }
    }
}