namespace JohnnyDemo.Model
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string InvoiceInfo { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CustomerDTO? Customer { get; set; }
    }
}
