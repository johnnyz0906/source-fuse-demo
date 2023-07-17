using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;

namespace JohnnyDemo.Repository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerDTO>> SearchAsync(string? firstName = null, string? lastName = null, string? email = null, string? phoneNumber = null, int skip = 0, int take = 50, bool includeOrders = false, CancellationToken cancellationToken = default);
        Task<CustomerDTO?> GetAsync(int id, bool includeOrders = false, CancellationToken cancellationToken = default);
        Task<CustomerDTO?> CreateAsync(CustomerCreateRequst createRequest, CancellationToken cancellationToken = default);
        Task<CustomerDTO?> UpdateAsync(int id, CustomerUpdateRequest updateRequest, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
