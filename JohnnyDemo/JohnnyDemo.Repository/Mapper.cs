using JohnnyDemo.Model;
using JohnnyDemo.Repository.DomainModels;
using JohnnyDemo.Repository.Models;

namespace JohnnyDemo.Repository
{
    internal static class Mapper
    {
        internal static CustomerDTO? Translate(Customer? customer, bool includeOrders = false)
        {
            if (customer == null) return null;
            var customerDTO = new CustomerDTO()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
            };
            if (includeOrders)
            {
                foreach (var order in customer.Orders)
                {
                    customerDTO.Orders.Add(Translate(order)!);
                }
            }
            return customerDTO;
        }
        internal static OrderDTO? Translate(Order? order, bool includeCustomer = false)
        {
            if (order == null) return null;
            var orderDTO = new OrderDTO
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                InvoiceInfo = order.InvoiceInfo,
                OrderInfo = order.OrderInfo,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
            if (includeCustomer) orderDTO.Customer = Translate(order.Customer);

            return orderDTO;
        }
    }
}
