using System.ComponentModel.DataAnnotations;

namespace Oswald_POS.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Color { get; set; } = "#4f46e5";

        public List<Product> Products { get; set; } = new();
    }
}