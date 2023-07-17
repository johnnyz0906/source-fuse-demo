using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;

namespace JohnnyDemo.BusinessLogic
{
    public interface IOrderCommandManager
    {
        Task<OrderDTO?> CreateAsync(int customerId, OrderCreateRequest request, CancellationToken cancellationToken = default);
        Task<OrderDTO?> UpdateAsync(int customerId, int orderId, OrderUpdateRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int customerId, int orderId, CancellationToken cancellationToken = default);
    }
}
