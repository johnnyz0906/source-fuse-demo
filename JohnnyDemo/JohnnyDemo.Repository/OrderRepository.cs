using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;
using JohnnyDemo.Repository.Context;
using JohnnyDemo.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JohnnyDemo.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ILogger _logger;
        private readonly JohnnyDemoContext _context;
        public OrderRepository(ILogger<OrderRepository> logger, JohnnyDemoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<OrderDTO?> CreateAsync(int customerId, OrderCreateRequest createRequest, CancellationToken cancellationToken = default)
        {
            var order = new Order
            {
                CustomerId = customerId,
                InvoiceInfo = createRequest.InvoiceInfo,
                OrderInfo = createRequest.OrderInfo,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Orders.Add(order);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Mapper.Translate(order);
            }
            return null;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders.FindAsync(id, cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Tried to delete the non-exist order with Id ({id})", id);
                throw new InvalidOperationException("Order with the specified Id does not exist");
            }
            _context.Orders.Remove(order);

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<OrderDTO?> GetAsync(int id, bool includeCustomer = false, CancellationToken cancellationToken = default)
        {
            Order? order;
            if (includeCustomer) order = await _context.Orders.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == id);
            else order = await _context.Orders.FindAsync(id, cancellationToken);

            return Mapper.Translate(order, includeCustomer);
        }

        public async Task<IEnumerable<OrderDTO>> SearchAsync(int? customerId = null, bool includeCustomer = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            var query = _context.Orders.AsQueryable();
            if (customerId != null)
                query = query.Where(x => x.CustomerId == customerId);
            if (includeCustomer)
                query = query.Include(x => x.Customer);

            var list = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);

            var listDTO = new List<OrderDTO>();

            foreach (var order in list)
            {
                listDTO.Add(Mapper.Translate(order, includeCustomer)!);
            }

            return listDTO;
        }

        public async Task<OrderDTO?> UpdateAsync(int id, OrderUpdateRequest updateRequest, CancellationToken cancellationToken = default)
        {
            if (id != updateRequest.Id)
            {
                throw new InvalidOperationException("Specified Id in payload is invalid");
            }
            var order = await _context.Orders.FindAsync(id, cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Tried to update the non-exist order with Id ({id})", id);
                throw new InvalidOperationException("Order with the specified Id does not exist");
            }

            if (updateRequest.InvoiceInfo != null)
            {
                order.InvoiceInfo = updateRequest.InvoiceInfo;
            }
            if (updateRequest.OrderInfo != null)
            {
                order.OrderInfo = updateRequest.OrderInfo;
            }

            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Mapper.Translate(order);
        }
    }
}
