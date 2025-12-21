using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FCG.Game.API.Controllers;
using FCG.Game.Application.Clients;
using FCG.Game.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FCG.Game.Tests
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IPaymentApiClient> _paymentClientMock;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _paymentClientMock = new Mock<IPaymentApiClient>();
            _controller = new PaymentsController(_paymentClientMock.Object);

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };
        }

        [Fact]
        public async Task ConsultarPagamento_ReturnsOk_WhenClientReturnsResponse()
        {
            var paymentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var resp = new PaymentResponseDto { PaymentId = paymentId, StatusPayment = "Completed", Success = true };

            _paymentClientMock.Setup(c => c.ConsultPaymentAsync(paymentId, userId)).ReturnsAsync(resp);

            var result = await _controller.ConsultarPagamento(paymentId, userId);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(resp);
        }

        [Fact]
        public async Task ConsultarPagamento_ReturnsNotFound_WhenClientReturnsNull()
        {
            var paymentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _paymentClientMock.Setup(c => c.ConsultPaymentAsync(paymentId, userId)).ReturnsAsync((PaymentResponseDto?)null);

            var result = await _controller.ConsultarPagamento(paymentId, userId);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CancelarPagamento_ReturnsOk_WhenClientReturnsResponse()
        {
            var request = new PaymentRequestDto { PaymentId = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var resp = new PaymentResponseDto { PaymentId = request.PaymentId, Success = true, StatusPayment = "CANCELED" };

            _paymentClientMock.Setup(c => c.CancelPaymentAsync(request)).ReturnsAsync(resp);

            var result = await _controller.CancelarPagamento(request);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(resp);
        }

        [Fact]
        public async Task CancelarPagamento_ReturnsBadRequest_WhenClientReturnsNull()
        {
            var request = new PaymentRequestDto { PaymentId = Guid.NewGuid(), UserId = Guid.NewGuid() };

            _paymentClientMock.Setup(c => c.CancelPaymentAsync(request)).ReturnsAsync((PaymentResponseDto?)null);

            var result = await _controller.CancelarPagamento(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
