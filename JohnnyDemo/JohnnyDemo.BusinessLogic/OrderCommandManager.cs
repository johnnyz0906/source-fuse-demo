using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;
using JohnnyDemo.Repository;
using Microsoft.Extensions.Logging;

namespace JohnnyDemo.BusinessLogic
{
    public class OrderCommandManager : IOrderCommandManager
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;
        public OrderCommandManager(ICustomerRepository customerRepository, IOrderRepository orderRepository, ILogger<OrderCommandManager> logger)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderDTO?> CreateAsync(int customerId, OrderCreateRequest request, CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetAsync(customerId, false, cancellationToken);
            if (customer == null)
            {
                _logger.LogWarning("Tried to create an order for non-exist customer({customerId})", customerId);
                throw new InvalidOperationException("Specified customer does not exit");
            }
            return await _orderRepository.CreateAsync(customerId, request, cancellationToken);
        }
        public async Task<OrderDTO?> UpdateAsync(int customerId, int orderId, OrderUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetAsync(orderId, false, cancellationToken);
            if (order == null)
            {
                _logger.LogWarning("Tried to update non-exist order({orderId})", orderId);
                throw new InvalidOperationException("Tried to update non-exist order({orderId})");
            }
            if (customerId != order.CustomerId)
            {
                _logger.LogWarning("Tried to update another customer's order");
                throw new InvalidOperationException("Customer does not have specified order");
            }

            return await _orderRepository.UpdateAsync(orderId, request, cancellationToken);
        }
        public async Task<bool> DeleteAsync(int customerId, int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetAsync(orderId, false, cancellationToken);
            if (order == null)
            {
                _logger.LogWarning("Tried to update non-exist order({orderId})", orderId);
                throw new InvalidOperationException("Tried to update non-exist order({orderId})");
            }
            if (customerId != order.CustomerId)
            {
                _logger.LogWarning("Tried to update another customer's order");
                throw new InvalidOperationException("Customer does not have specified order");
            }

            return await _orderRepository.DeleteAsync(orderId, cancellationToken);
        }
    }
}
