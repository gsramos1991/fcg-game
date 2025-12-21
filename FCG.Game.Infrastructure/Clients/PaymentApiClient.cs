using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FCG.Game.Application.Clients;
using FCG.Game.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FCG.Game.Infrastructure.Clients
{
    public class PaymentApiClient : IPaymentApiClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly ILogger<PaymentApiClient> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public PaymentApiClient(HttpClient http, IConfiguration configuration, ILogger<PaymentApiClient> logger)
        {
            _http = http;
            _logger = logger;
            _baseUrl = configuration["PaymentApi:BaseUrl"] ?? string.Empty;
        }

        public async Task<PaymentResponseDto?> ConsultPaymentAsync(Guid paymentId, Guid userId)
        {
            var url = string.IsNullOrEmpty(_baseUrl)
                ? $"api/Payment/ConsultarPagamento/{paymentId}/{userId}"
                : new Uri(new Uri(_baseUrl.TrimEnd('/')), $"api/Payment/ConsultarPagamento/{paymentId}/{userId}").ToString();

            _logger.LogDebug("ConsultPaymentAsync GET {Url}", url);

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("ConsultPaymentAsync returned {StatusCode}", res.StatusCode);
                return null;
            }

            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponseDto>(json, _jsonOptions);
        }

        public async Task<PaymentResponseDto?> CancelPaymentAsync(PaymentRequestDto request)
        {
            var url = string.IsNullOrEmpty(_baseUrl)
                ? "api/Payment/CancelarPagamento"
                : new Uri(new Uri(_baseUrl.TrimEnd('/')), "api/Payment/CancelarPagamento").ToString();

            var payload = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            _logger.LogDebug("CancelPaymentAsync POST {Url} payload={Payload}", url, payload);

            var res = await _http.PostAsync(url, content);
            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("CancelPaymentAsync returned {StatusCode}", res.StatusCode);
                return null;
            }

            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponseDto>(json, _jsonOptions);
        }
    }
}
