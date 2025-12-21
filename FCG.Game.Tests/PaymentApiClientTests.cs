using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using FCG.Game.Application.DTOs;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FCG.Game.Tests
{
    internal class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_responder(request));
    }

    public class PaymentApiClientTests
    {
        [Fact]
        public async Task ConsultPaymentAsync_ReturnsPaymentResponse_WhenHttpOk()
        {
            var paymentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var expected = new PaymentResponseDto { PaymentId = paymentId, Success = true, StatusPayment = "OK" };
            var json = JsonSerializer.Serialize(expected);

            var handler = new FakeHandler(req =>
            {
                req.RequestUri.ToString().Should().Contain($"/api/Payment/ConsultarPagamento/{paymentId}/{userId}");
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            });

            var client = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string>("PaymentApi:BaseUrl", "http://test/") }).Build();
            var logger = NullLogger<FCG.Game.Infrastructure.Clients.PaymentApiClient>.Instance;

            var paymentClient = new FCG.Game.Infrastructure.Clients.PaymentApiClient(client, config, logger);

            var result = await paymentClient.ConsultPaymentAsync(paymentId, userId);

            result.Should().NotBeNull();
            result!.PaymentId.Should().Be(paymentId);
        }

        [Fact]
        public async Task CancelPaymentAsync_ReturnsPaymentResponse_WhenHttpOk()
        {
            var reqDto = new PaymentRequestDto { PaymentId = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var expected = new PaymentResponseDto { PaymentId = reqDto.PaymentId, Success = true, StatusPayment = "CANCELED" };
            var json = JsonSerializer.Serialize(expected);

            var handler = new FakeHandler(req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri.ToString().Should().Contain("/api/Payment/CancelarPagamento");
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            });

            var client = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string>("PaymentApi:BaseUrl", "http://test/") }).Build();
            var logger = NullLogger<FCG.Game.Infrastructure.Clients.PaymentApiClient>.Instance;

            var paymentClient = new FCG.Game.Infrastructure.Clients.PaymentApiClient(client, config, logger);

            var result = await paymentClient.CancelPaymentAsync(reqDto);

            result.Should().NotBeNull();
            result!.PaymentId.Should().Be(reqDto.PaymentId);
        }
    }
}
