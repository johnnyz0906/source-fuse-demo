using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;
using JohnnyDemo.Repository;
using Microsoft.Extensions.Logging;

namespace JohnnyDemo.BusinessLogic
{
    public class CustomerCommandManager : ICustomerCommandManager
    {
        private readonly ILogger _logger;
        private readonly ICustomerRepository _repository;
        public CustomerCommandManager(ILogger<CustomerCommandManager> logger, ICustomerRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<CustomerDTO?> CreateAsync(CustomerCreateRequst createRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.CreateAsync(createRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create customer");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.DeleteAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete customer({id})", id);
                throw;
            }
        }

        public async Task<CustomerDTO?> UpdateAsync(int id, CustomerUpdateRequest updateRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.UpdateAsync(id, updateRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update customer({id})", id);
                throw;
            }
        }
    }
}
