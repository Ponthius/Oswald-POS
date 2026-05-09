namespace Oswald_POS.ViewModels
{
    public class SaleRequest
    {
        public List<SaleRequestItem> Items { get; set; } = new();

        public decimal AmountPaid { get; set; }

        public string PaymentMethod { get; set; } = "Cash";

        public int? CustomerId { get; set; }
    }

    public class SaleRequestItem
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}