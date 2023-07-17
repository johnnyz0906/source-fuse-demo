using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;

namespace JohnnyDemo.Client
{
    public interface IDemoClient
    {
        Task<CustomerDTO?> GetCustomerAsync(int customerId, bool includeOrders = false, CancellationToken cancellation = default);
        Task<IEnumerable<CustomerDTO>?> SearchCustomerAsync(string? firstName, string? lastName, string? email, string? phoneNumber, bool includeOrders = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
        Task<CustomerDTO?> CreateCustomerAsync(CustomerCreateRequst request, CancellationToken cancellation = default);
        Task<CustomerDTO?> UpdateCustomerAsync(int customerId, CustomerUpdateRequest request, CancellationToken cancellation = default);
        Task<bool> DeleteCustomerAsync(int customerId, CancellationToken cancellation = default);
        Task<OrderDTO?> GetCustomerOrderAsync(int customerId, int orderId, bool includeCustomer = false, CancellationToken cancellation = default);
        Task<IEnumerable<OrderDTO>?> GetCustomerOrdersAsync(int customerId, bool includeCustomer = false, CancellationToken cancellation = default);
        Task<OrderDTO?> CreateCustomerOrderAsync(int customerId, OrderCreateRequest request, CancellationToken cancellation = default);
        Task<OrderDTO?> UpdateCustomerOrderAsync(int customerId, int orderId, OrderUpdateRequest request, CancellationToken cancellation = default);
        Task<bool> DeleteCustomerOrderAsync(int customerId, int orderId, CancellationToken cancellation = default);
    }
}
