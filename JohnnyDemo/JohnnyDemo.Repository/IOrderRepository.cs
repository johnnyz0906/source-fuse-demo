using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;

namespace JohnnyDemo.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDTO>> SearchAsync(int? customerId = null, bool includeCustomer = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
        Task<OrderDTO?> GetAsync(int id, bool includeCustomer = false, CancellationToken cancellationToken = default);
        Task<OrderDTO?> CreateAsync(int customerId, OrderCreateRequest createRequest, CancellationToken cancellationToken = default);
        Task<OrderDTO?> UpdateAsync(int id, OrderUpdateRequest updateRequest, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
