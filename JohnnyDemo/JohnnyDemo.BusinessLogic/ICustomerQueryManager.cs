using JohnnyDemo.Model;

namespace JohnnyDemo.BusinessLogic
{
    public interface ICustomerQueryManager
    {
        Task<CustomerDTO?> GetAsync(int id, bool includeOrders = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerDTO>> SearchAsync(string? firstName = null, string? lastName = null, string? email = null, string? phoneNumber = null, int skip = 0, int take = 50, bool includeOrders = false, CancellationToken cancellationToken = default);
    }
}
