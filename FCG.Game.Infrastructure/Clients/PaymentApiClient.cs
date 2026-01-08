using FCG.Game.Application.Clients;
using FCG.Game.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        }

        public async Task<PaymentResponseDto?> ConsultPaymentAsync(Guid paymentId, Guid userId)
        {
            // 1. Construção limpa da URL
            var relativePath = $"api/Payment/ConsultarPagamento/{paymentId}/{userId}";

            _logger.LogDebug("ConsultPaymentAsync GET {RelativePath} (Base: {BaseUrl})", relativePath, _http.BaseAddress);

            try
            {
                // 2. Uso do GetFromJsonAsync: Ele já faz o Get, verifica o StatusCode e desserializa
                // Se a base URL estiver configurada no HttpClient (via AddHttpClient), basta passar o path relativo.
                var response = await _http.GetFromJsonAsync<PaymentResponseDto>(relativePath, _jsonOptions);

                return response;
            }
            catch (HttpRequestException ex)
            {
                // Loga falhas de conexão ou status de erro (4xx, 5xx)
                _logger.LogWarning(ex, "Erro ao consultar pagamento {PaymentId}. Status: {StatusCode}", paymentId, ex.StatusCode);
                return null;
            }
            catch (JsonException ex)
            {
                // Loga erro se o JSON retornado for inválido
                _logger.LogError(ex, "Erro ao desserializar resposta de pagamento para {PaymentId}", paymentId);
                return null;
            }
        }

        public async Task<PaymentResponseDto?> CancelPaymentAsync(PaymentRequestDto request)
        {
            const string relativePath = "api/Payment/CancelarPagamento";

            _logger.LogDebug("CancelPaymentAsync POST {RelativePath} para o ID {PaymentId}",
                relativePath, request.PaymentId); // Ajuste 'PaymentId' conforme seu DTO

            try
            {
                // Envia o objeto 'request' como JSON e aguarda a resposta
                var response = await _http.PostAsJsonAsync(relativePath, request, _jsonOptions);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("CancelPaymentAsync falhou. Status: {StatusCode}. Erro: {Error}",
                        response.StatusCode, errorContent);
                    return null;
                }

                // Lê e desserializa o retorno
                return await response.Content.ReadFromJsonAsync<PaymentResponseDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao cancelar pagamento");
                return null;
            }
        }
    }
}
