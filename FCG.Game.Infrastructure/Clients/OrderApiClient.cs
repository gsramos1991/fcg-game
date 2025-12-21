using FCG.Game.Application.Clients;
using FCG.Game.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FCG.Game.Infrastructure.Clients
{
    public class OrderApiClient : IOrderApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _orderApiUrl;

        public OrderApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _orderApiUrl = configuration["OrderApi:Url"] ?? throw new InvalidOperationException("Order API URL is not configured in appsettings.json.");
        }

        public async Task<Guid> CreateOrderAsync(OrderApiRequest orderRequest)
        {
            var response = await _httpClient.PostAsJsonAsync(_orderApiUrl, orderRequest);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OrderApiResponse>();
            return result?.paymentId ?? Guid.Empty;
        }
    }

    internal class OrderApiResponse
    {
        public Guid paymentId { get; set; }
        public string statusPayment { get; set; } = string.Empty;
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public DateTime? processedAt { get; set; }
    }
}