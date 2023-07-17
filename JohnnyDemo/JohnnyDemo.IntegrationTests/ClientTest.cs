using AutoFixture;
using JohnnyDemo.Client;
using JohnnyDemo.Model.Requests;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace JohnnyDemo.IntegrationTests
{
    public class ClientTest
    {
        private readonly Mock<ILogger<DemoClient>> _loggerMock;
        private readonly IFixture _fixture;
        private readonly IDemoClient _client;
        private readonly IDemoClient _invalidKeyClient;
        private readonly string _apiKey;

        public ClientTest()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<DemoClient>>();
            _apiKey = _fixture.Create<string>();

            var httpClient = Helpers.GetClient(new HashSet<string> { _apiKey });

            _client = new DemoClient(httpClient, _loggerMock.Object, _apiKey);
            _invalidKeyClient = new DemoClient(httpClient, _loggerMock.Object, _fixture.Create<string>());
        }
        [Fact]
        public async Task CreateCustomer_Unauthorized()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await _invalidKeyClient.CreateCustomerAsync(request));
            Assert.NotNull(exception);
            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var result = await _client.CreateCustomerAsync(request);

            Assert.NotNull(result);
            Assert.Equal(request.FirstName, result.FirstName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.PhoneNumber, result.PhoneNumber);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.Address, result.Address);

            var getResult = await _client.GetCustomerAsync(result.Id);
            Assert.Equivalent(result, getResult);
        }

        [Fact]
        public async Task UpdateCustomer()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var result = await _client.CreateCustomerAsync(request);

            Assert.NotNull(result);


            var updateRequest = _fixture.Create<CustomerUpdateRequest>();
            updateRequest.Id = result.Id;

            var updatedResult = await _client.UpdateCustomerAsync(updateRequest.Id, updateRequest);
            Assert.NotNull(updatedResult);

            Assert.Equal(updateRequest.FirstName, updatedResult.FirstName);
            Assert.Equal(updateRequest.LastName, updatedResult.LastName);
            Assert.Equal(updateRequest.PhoneNumber, updatedResult.PhoneNumber);
            Assert.Equal(updateRequest.Email, updatedResult.Email);
            Assert.Equal(updateRequest.Address, updatedResult.Address);

            var getResult = await _client.GetCustomerAsync(result.Id);
            Assert.Equivalent(updateRequest, getResult);
        }
        [Fact]
        public async Task UpdateCustomer_Exception()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var result = await _client.CreateCustomerAsync(request);

            Assert.NotNull(result);


            var updateRequest = _fixture.Create<CustomerUpdateRequest>();
            updateRequest.Id = result.Id;

            var ex = await Assert.ThrowsAsync<HttpRequestException>(async () => await _client.UpdateCustomerAsync(_fixture.Create<int>(), updateRequest));

            Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        }
        [Fact]
        public async Task DeleteCustomer()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var result = await _client.CreateCustomerAsync(request);

            Assert.NotNull(result);

            var getResult = await _client.GetCustomerAsync(result.Id);
            Assert.NotNull(getResult);

            await _client.DeleteCustomerAsync(result.Id);

            var get2Result = await _client.GetCustomerAsync(result.Id);
            Assert.Null(get2Result);
        }
        [Fact]
        public async Task SearchCustomer()
        {
            var requests = _fixture.CreateMany<CustomerCreateRequst>();
            foreach (var request in requests)
            {
                await _client.CreateCustomerAsync(request);
            }

            var result = _client.SearchCustomerAsync(null, null, null, null);
            Assert.NotNull(result);
            Assert.Equal(3, requests.Count());

            var firstOne = await _client.SearchCustomerAsync(requests.First().FirstName, null, null, null);
            Assert.NotNull(firstOne);
            Assert.Single(firstOne);
            Assert.Equal(firstOne.First().FirstName, requests.First().FirstName);
            Assert.Equal(firstOne.First().LastName, requests.First().LastName);
            Assert.Equal(firstOne.First().Email, requests.First().Email);
            Assert.Equal(firstOne.First().PhoneNumber, requests.First().PhoneNumber);
            Assert.Equal(firstOne.First().Address, requests.First().Address);
        }

        [Fact]
        public async Task GetCustomer_WithOrders()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var result = await _client.CreateCustomerAsync(request);

            Assert.NotNull(result);

            var orderRequets = _fixture.CreateMany<OrderCreateRequest>();
            foreach (var order in orderRequets)
            {
                await _client.CreateCustomerOrderAsync(result.Id, order);
            }

            var getResult = await _client.GetCustomerAsync(result.Id, true);
            Assert.NotNull(getResult);
            Assert.Equal(3, getResult.Orders.Count);

            var ordersResult = await _client.GetCustomerOrdersAsync(result.Id);
            Assert.NotNull(ordersResult);
            Assert.Equal(3, ordersResult.Count());

            for (var i = 0; i < 3; i++)
            {
                Assert.Equivalent(getResult.Orders.ElementAt(i), ordersResult.ElementAt(i));
            }
        }
        [Fact]
        public async Task UpdateCustomerOrder()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var result = await _client.CreateCustomerAsync(request);

            Assert.NotNull(result);

            var orderCreateRequest = _fixture.Create<OrderCreateRequest>();

            var order = await _client.CreateCustomerOrderAsync(result.Id, orderCreateRequest);
            Assert.NotNull(order);

            var orderUpdateRequest = _fixture.Create<OrderUpdateRequest>();
            orderUpdateRequest.Id = order.Id;

            var updated = await _client.UpdateCustomerOrderAsync(result.Id, order.Id, orderUpdateRequest);
            Assert.NotNull(updated);

            Assert.Equal(orderUpdateRequest.InvoiceInfo, updated.InvoiceInfo);
            Assert.Equal(orderUpdateRequest.OrderInfo, updated.OrderInfo);
        }
        [Fact]
        public async Task UpdateCustomerOrder_Exception()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var result = await _client.CreateCustomerAsync(request);

            Assert.NotNull(result);

            var orderCreateRequest = _fixture.Create<OrderCreateRequest>();

            var order = await _client.CreateCustomerOrderAsync(result.Id, orderCreateRequest);
            Assert.NotNull(order);

            var orderUpdateRequest = _fixture.Create<OrderUpdateRequest>();

            var ex = await Assert.ThrowsAsync<HttpRequestException>(async () => await _client.UpdateCustomerOrderAsync(result.Id, order.Id, orderUpdateRequest));

            Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        }
        [Fact]
        public async Task UpdateCustomerOrder_WrongCustomerException()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var customer = await _client.CreateCustomerAsync(request);
            Assert.NotNull(customer);

            var request2 = _fixture.Create<CustomerCreateRequst>();
            var customer2 = await _client.CreateCustomerAsync(request2);
            Assert.NotNull(customer2);

            var orderCreateRequest = _fixture.Create<OrderCreateRequest>();

            var order = await _client.CreateCustomerOrderAsync(customer.Id, orderCreateRequest);
            Assert.NotNull(order);

            var orderUpdateRequest = _fixture.Create<OrderUpdateRequest>();

            var ex = await Assert.ThrowsAsync<HttpRequestException>(async () => await _client.UpdateCustomerOrderAsync(customer2.Id, order.Id, orderUpdateRequest));

            Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        }
        [Fact]
        public async Task GetCustomerOrder_Exception()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var customer = await _client.CreateCustomerAsync(request);
            Assert.NotNull(customer);

            var request2 = _fixture.Create<CustomerCreateRequst>();
            var customer2 = await _client.CreateCustomerAsync(request2);
            Assert.NotNull(customer2);

            var orderCreateRequest = _fixture.Create<OrderCreateRequest>();

            var order = await _client.CreateCustomerOrderAsync(customer.Id, orderCreateRequest);
            Assert.NotNull(order);

            var ex = await Assert.ThrowsAsync<HttpRequestException>(async () => await _client.GetCustomerOrderAsync(customer2.Id, order.Id));

            Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        }
        [Fact]
        public async Task DeleteCustomerOrder()
        {
            var request = _fixture.Create<CustomerCreateRequst>();
            var customer = await _client.CreateCustomerAsync(request);
            Assert.NotNull(customer);

            var orderCreateRequest = _fixture.Create<OrderCreateRequest>();

            var order = await _client.CreateCustomerOrderAsync(customer.Id, orderCreateRequest);
            Assert.NotNull(order);

            var orders = await _client.GetCustomerOrdersAsync(customer.Id);
            Assert.NotNull(orders);
            Assert.Single(orders);

            await _client.DeleteCustomerOrderAsync(customer.Id, order.Id);

            orders = await _client.GetCustomerOrdersAsync(customer.Id);
            Assert.NotNull(orders);
            Assert.Empty(orders);

        }
    }
}