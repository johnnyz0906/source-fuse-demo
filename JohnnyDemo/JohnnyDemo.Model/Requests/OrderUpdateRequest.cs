namespace JohnnyDemo.Model.Requests
{
    public class OrderUpdateRequest
    {
        public int Id { get; set; }
        public string? InvoiceInfo { get; set; }
        public string? OrderInfo { get; set; }

    }
}
