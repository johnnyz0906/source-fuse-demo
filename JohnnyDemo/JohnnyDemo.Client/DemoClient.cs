using JohnnyDemo.Model;
using JohnnyDemo.Model.Requests;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;

namespace JohnnyDemo.Client
{
    public class DemoClient : IDemoClient
    {
        private readonly string _baseUri = "api";
        private readonly string _apiKey;
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _jsonOptins;

        public DemoClient(HttpClient httpClient, ILogger<DemoClient> logger, string apiKey)
        {
            _client = httpClient;
            _logger = logger;
            _apiKey = apiKey;
            _jsonOptins = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }
        public async Task<CustomerDTO?> GetCustomerAsync(int customerId, bool includeOrders = false, CancellationToken cancellation = default)
        {
            var paramters = new Dictionary<string, string?>()
            {
                { "includeOrders", includeOrders.ToString() }
            };
            var url = CreateUrl($"customers/{customerId}", paramters);

            return await GetAsync<CustomerDTO>(url, cancellation);
        }
        public async Task<IEnumerable<CustomerDTO>?> SearchCustomerAsync(string? firstName, string? lastName, string? email, string? phoneNumber, bool includeOrders = false, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            var paramters = new Dictionary<string, string?>()
            {
                { "firstName", firstName },
                { "lastName", lastName },
                { "email", email },
                { "phoneNumber", phoneNumber },
                { "includeOrders", includeOrders.ToString() },
                { "skip", skip.ToString() },
                { "take", take.ToString() },
            };
            var url = CreateUrl($"customers", paramters);

            return await GetAsync<IEnumerable<CustomerDTO>>(url, cancellationToken);
        }
        public async Task<CustomerDTO?> CreateCustomerAsync(CustomerCreateRequst request, CancellationToken cancellation = default)
        {
            var url = CreateUrl("customers");
            return await PostAsync<CustomerDTO>(url, request, cancellation);
        }
        public async Task<CustomerDTO?> UpdateCustomerAsync(int customerId, CustomerUpdateRequest request, CancellationToken cancellation = default)
        {
            var url = CreateUrl($"customers/{customerId}");
            return await PutAsync<CustomerDTO>(url, request, cancellation);
        }
        public async Task<bool> DeleteCustomerAsync(int customerId, CancellationToken cancellation = default)
        {
            var url = CreateUrl($"customers/{customerId}");
            return await DeleteAsync(url, cancellation);
        }

        public async Task<OrderDTO?> GetCustomerOrderAsync(int customerId, int orderId, bool includeCustomer = false, CancellationToken cancellation = default)
        {
            var paramters = new Dictionary<string, string?>()
            {
                { "includeCustomer", includeCustomer.ToString() }
            };

            var url = CreateUrl($"customers/{customerId}/orders/{orderId}", paramters);
            return await GetAsync<OrderDTO>(url, cancellation);
        }
        public async Task<IEnumerable<OrderDTO>?> GetCustomerOrdersAsync(int customerId, bool includeCustomer = false, CancellationToken cancellation = default)
        {
            var paramters = new Dictionary<string, string?>()
            {
                { "includeCustomer", includeCustomer.ToString() }
            };
            var url = CreateUrl($"customers/{customerId}/orders", paramters);
            return await GetAsync<IEnumerable<OrderDTO>>(url, cancellation);
        }
        public async Task<OrderDTO?> CreateCustomerOrderAsync(int customerId, OrderCreateRequest request, CancellationToken cancellation = default)
        {
            var url = CreateUrl($"customers/{customerId}/orders");
            return await PostAsync<OrderDTO>(url, request, cancellation);
        }
        public async Task<OrderDTO?> UpdateCustomerOrderAsync(int customerId, int orderId, OrderUpdateRequest request, CancellationToken cancellation = default)
        {
            var url = CreateUrl($"customers/{customerId}/orders/{orderId}");
            return await PutAsync<OrderDTO>(url, request, cancellation);
        }
        public async Task<bool> DeleteCustomerOrderAsync(int customerId, int orderId, CancellationToken cancellation = default)
        {
            var url = CreateUrl($"customers/{customerId}/orders/{orderId}");
            return await DeleteAsync(url, cancellation);
        }
        private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Request: GET => {url}", url);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("X-API-Key", _apiKey);

            var response = await _client.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default(T);

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptins, cancellationToken);
        }
        private async Task<T?> PostAsync<T>(string url, object payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Request: POST => {url}", url);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("X-API-Key", _apiKey);
            requestMessage.Content = JsonContent.Create(payload);

            var response = await _client.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default(T);

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptins, cancellationToken);
        }
        private async Task<T?> PutAsync<T>(string url, object payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Request: PUT => {url}", url);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            requestMessage.Headers.Add("X-API-Key", _apiKey);
            requestMessage.Content = JsonContent.Create(payload);

            var response = await _client.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default(T);

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptins, cancellationToken);
        }
        private async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Request: DELETE => {url}", url);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
            requestMessage.Headers.Add("X-API-Key", _apiKey);

            var response = await _client.SendAsync(requestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();

            return true;
        }
        private string CreateUrl(string? operation = null, Dictionary<string, string?> paramters = null)
        {
            var url = new StringBuilder(_baseUri);
            if (!string.IsNullOrEmpty(operation))
                url.Append($"/{operation}");
            if (paramters != null && paramters.Any(pair => pair.Value != null))
            {
                url.Append("?");
                var pairs = paramters.Where(x => x.Value != null).Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}").ToList();
                url.Append(string.Join("&", pairs));
            }

            return url.ToString();
        }
    }
}
