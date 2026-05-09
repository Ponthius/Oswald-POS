using System.ComponentModel.DataAnnotations;

namespace Oswald_POS.Models
{
    public class Worker
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        public string Role { get; set; } = "Cashier";
    }
}