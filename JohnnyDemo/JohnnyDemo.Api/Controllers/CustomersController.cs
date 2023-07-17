using JohnnyDemo.Api.Filters;
using JohnnyDemo.BusinessLogic;
using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JohnnyDemo.Api.Controllers
{
    /// <summary>
    /// Controller to manage customers
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyRequired]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerQueryManager _queryManager;
        private readonly ICustomerCommandManager _commandManager;
        private readonly ILogger _logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="queryManager"></param>
        /// <param name="commandManager"></param>
        /// <param name="logger"></param>
        public CustomersController(ICustomerQueryManager queryManager, ICustomerCommandManager commandManager, ILogger<CustomersController> logger)
        {
            _queryManager = queryManager;
            _commandManager = commandManager;
            _logger = logger;
        }
        /// <summary>
        /// Retrieve list of customers 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="includeOrders"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> SearchAsync(string? firstName, string? lastName, string? email, string? phoneNumber, bool includeOrders = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            return Ok(await _queryManager.SearchAsync(firstName, lastName, email, phoneNumber, skip, take, includeOrders, cancellationToken));
        }
        /// <summary>
        /// Retrive an customer with ID
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="includeOrders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{customerId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<CustomerDTO?>> GetAsync(int customerId, bool includeOrders = false, CancellationToken cancellationToken = default)
        {
            return await _queryManager.GetAsync(customerId, includeOrders, cancellationToken);
        }
        /// <summary>
        /// Creates new customer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<CustomerDTO?>> CreateAsync([FromBody] CustomerCreateRequst request, CancellationToken cancellationToken = default)
        {
            return await _commandManager.CreateAsync(request, cancellationToken);
        }
        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{customerId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<CustomerDTO?>> UpdateAsync(int customerId, [FromBody] CustomerUpdateRequest request, CancellationToken cancellationToken = default)
        {
            return await _commandManager.UpdateAsync(customerId, request, cancellationToken);
        }
        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{customerId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> DeleteAsync(int customerId, CancellationToken cancellationToken = default)
        {
            if (await _commandManager.DeleteAsync(customerId, cancellationToken))
                return Ok();
            else return BadRequest();
        }
    }
}
