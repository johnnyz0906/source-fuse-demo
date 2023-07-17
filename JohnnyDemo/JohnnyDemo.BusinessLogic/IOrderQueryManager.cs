using JohnnyDemo.Model;

namespace JohnnyDemo.BusinessLogic
{
    public interface IOrderQueryManager
    {
        Task<IEnumerable<OrderDTO>> SearchAsync(int? customerId = null, bool includeCustomer = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
        Task<OrderDTO?> GetAsync(int customerId, int orderId, bool includeCustomer = false, CancellationToken cancellationToken = default);
    }
}
