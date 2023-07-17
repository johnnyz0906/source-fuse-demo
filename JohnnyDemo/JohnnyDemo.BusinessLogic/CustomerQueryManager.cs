using JohnnyDemo.Model;
using JohnnyDemo.Repository;
using Microsoft.Extensions.Logging;

namespace JohnnyDemo.BusinessLogic
{
    public class CustomerQueryManager : ICustomerQueryManager
    {
        private readonly ILogger _logger;
        private readonly ICustomerRepository _repository;
        public CustomerQueryManager(ILogger<CustomerQueryManager> logger, ICustomerRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<CustomerDTO?> GetAsync(int id, bool includeOrders = false, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.GetAsync(id, includeOrders, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get a customer({id})", id);
                throw;
            }
        }

        public async Task<IEnumerable<CustomerDTO>> SearchAsync(string? firstName = null, string? lastName = null, string? email = null, string? phoneNumber = null, int skip = 0, int take = 50, bool includeOrders = false, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.SearchAsync(firstName, lastName, email, phoneNumber, skip, take, includeOrders, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search customers - firstName({firstName}), lastName({lastname}), email({email}), phoneNumber({phoneNumber}), skip({skip}), take({take}), includeOrders({includeOrders})", firstName, lastName, email, phoneNumber, skip, take, includeOrders);
                throw;
            }
        }
    }
}
