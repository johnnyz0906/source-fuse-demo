using JohnnyDemo.Model;
using JohnnyDemo.Repository;
using Microsoft.Extensions.Logging;

namespace JohnnyDemo.BusinessLogic
{
    public class OrderQueryManager : IOrderQueryManager
    {
        private readonly ILogger _logger;
        private readonly IOrderRepository _repository;
        public OrderQueryManager(ILogger<OrderQueryManager> logger, IOrderRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public async Task<OrderDTO?> GetAsync(int customerId, int orderId, bool includeCustomer = false, CancellationToken cancellationToken = default)
        {
            OrderDTO? orderDTO;
            try
            {
                orderDTO = await _repository.GetAsync(orderId, includeCustomer, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get an order - customer({customerId}), order({orderId}), includeCustomer({includeCustomer})", customerId, orderId, includeCustomer);
                throw;
            }


            if (orderDTO != null && orderDTO.CustomerId != customerId)
            {
                _logger.LogWarning("Customer ({customerId}) tried to get other customer's order ({orderId})", customerId, orderId);
                throw new InvalidOperationException("Invalid customer");
            }
            return orderDTO;
        }

        public async Task<IEnumerable<OrderDTO>> SearchAsync(int? customerId = null, bool includeCustomer = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.SearchAsync(customerId, includeCustomer, skip, take, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search orders - customer({customerId}), includeCustomer({includeCustomer}), skip({skip}), take({take})", customerId, includeCustomer, skip, take);
                throw;
            }
        }
    }
}
