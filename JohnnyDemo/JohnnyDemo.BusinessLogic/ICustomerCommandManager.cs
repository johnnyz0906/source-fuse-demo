using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;

namespace JohnnyDemo.BusinessLogic
{
    public interface ICustomerCommandManager
    {
        Task<CustomerDTO?> CreateAsync(CustomerCreateRequst createRequest, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<CustomerDTO?> UpdateAsync(int id, CustomerUpdateRequest updateRequest, CancellationToken cancellationToken = default);
    }
}
