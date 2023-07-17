using JohnnyDemo.Api.Filters;
using JohnnyDemo.BusinessLogic;
using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JohnnyDemo.Api.Controllers
{
    /// <summary>
    /// Controller to manage specific customer's orders
    /// </summary>
    [Route("api/customers/{customerId}/[controller]")]
    [ApiController]
    [ApiKeyRequired]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrderQueryManager _queryManager;
        private readonly IOrderCommandManager _commandManager;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="queryManager"></param>
        /// <param name="commandManager"></param>
        public OrdersController(ILogger<OrdersController> logger, IOrderQueryManager queryManager, IOrderCommandManager commandManager)
        {
            _logger = logger;
            _queryManager = queryManager;
            _commandManager = commandManager;
        }
        /// <summary>
        /// Retrieve list of specified customer's orders
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="includeCustomer"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> SearchAsync(int customerId, bool includeCustomer = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            return Ok(await _queryManager.SearchAsync(customerId, includeCustomer, skip, take, cancellationToken));
        }
        /// <summary>
        /// Retrieve the specified customer's order with ID
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="orderId"></param>
        /// <param name="includeCustomer"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [HttpGet("{orderId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<OrderDTO?>> GetAsync(int customerId, int orderId, bool includeCustomer = false, CancellationToken cancellation = default)
        {
            return await _queryManager.GetAsync(customerId, orderId, includeCustomer, cancellation);
        }
        /// <summary>
        /// Create order for specified customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="request"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<OrderDTO?>> CreateAsync(int customerId, OrderCreateRequest request, CancellationToken cancellation = default)
        {
            return await _commandManager.CreateAsync(customerId, request, cancellation);
        }
        /// <summary>
        /// Update specified customer's order
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="orderId"></param>
        /// <param name="request"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [HttpPut("{orderId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<OrderDTO?>> UpdateAsync(int customerId, int orderId, OrderUpdateRequest request, CancellationToken cancellation = default)
        {
            return await _commandManager.UpdateAsync(customerId, orderId, request, cancellation);
        }
        /// <summary>
        /// Delete specified customer's order
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="orderId"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [HttpDelete("{orderId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteAsync(int customerId, int orderId, CancellationToken cancellation = default)
        {
            await _commandManager.DeleteAsync(customerId, orderId, cancellation);
            return Ok();
        }
    }
}
