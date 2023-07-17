using JohnnyDemo.Repository.DomainModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace JohnnyDemo.Repository.Models
{
    internal class Order : BaseEntity
    {
        public Order() { }
        public int CustomerId { get; set; }
        public string InvoiceInfo { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }
    }
}
