using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;
using JohnnyDemo.Repository.Context;
using JohnnyDemo.Repository.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JohnnyDemo.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ILogger _logger;
        private readonly JohnnyDemoContext _context;
        public CustomerRepository(ILogger<CustomerRepository> logger, JohnnyDemoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<CustomerDTO?> CreateAsync(CustomerCreateRequst createRequest, CancellationToken cancellationToken = default)
        {
            var customer = new Customer
            {
                FirstName = createRequest.FirstName,
                LastName = createRequest.LastName,
                Address = createRequest.Address,
                Email = createRequest.Email,
                PhoneNumber = createRequest.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Customers.Add(customer);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Mapper.Translate(customer);
            }
            return null;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var customer = await _context.Customers.FindAsync(id, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning("Tried to delete the non-exist customer with Id ({id})", id);
                throw new InvalidOperationException("Customer with the specified Id does not exist");
            }
            _context.Customers.Remove(customer);

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<CustomerDTO?> GetAsync(int id, bool includeOrders = false, CancellationToken cancellationToken = default)
        {
            Customer? customer;
            if (includeOrders) customer = await _context.Customers.Include(x => x.Orders).FirstOrDefaultAsync(x => x.Id == id);
            else customer = await _context.Customers.FindAsync(id, cancellationToken);

            return Mapper.Translate(customer, includeOrders);
        }

        public async Task<IEnumerable<CustomerDTO>> SearchAsync(string? firstName = null, string? lastName = null, string? email = null, string? phoneNumber = null, int skip = 0, int take = 50, bool includeOrders = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(x => x.FirstName == firstName);
            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(x => x.LastName == lastName);
            if (!string.IsNullOrEmpty(email))
                query = query.Where(x => x.Email == email);
            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(x => x.PhoneNumber == phoneNumber);
            if (includeOrders)
                query = query.Include(x => x.Orders);

            var list = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);

            var listDTO = new List<CustomerDTO>();

            foreach (var customer in list)
            {
                listDTO.Add(Mapper.Translate(customer, includeOrders)!);
            }

            return listDTO;
        }

        public async Task<CustomerDTO?> UpdateAsync(int id, CustomerUpdateRequest updateRequest, CancellationToken cancellationToken = default)
        {
            if (id != updateRequest.Id)
            {
                throw new InvalidOperationException("Specified Id in payload is invalid");
            }
            var customer = await _context.Customers.FindAsync(id, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning("Tried to update the non-exist customer with Id ({id})", id);
                throw new InvalidOperationException("Customer with the specified Id does not exist");
            }

            if (updateRequest.FirstName != null)
            {
                customer.FirstName = updateRequest.FirstName;
            }
            if (updateRequest.LastName != null)
            {
                customer.LastName = updateRequest.LastName;
            }
            if (updateRequest.Email != null)
            {
                customer.Email = updateRequest.Email;
            }
            if (updateRequest.PhoneNumber != null)
            {
                customer.PhoneNumber = updateRequest.PhoneNumber;
            }
            if (updateRequest.Address != null)
            {
                customer.Address = updateRequest.Address;
            }

            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Mapper.Translate(customer);
        }
    }
}
