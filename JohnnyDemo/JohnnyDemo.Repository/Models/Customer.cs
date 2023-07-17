using JohnnyDemo.Repository.Models;

namespace JohnnyDemo.Repository.DomainModels
{

    internal class Customer : BaseEntity
    {
        public Customer() { }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
