using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FCG.Game.API.Controllers;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FCG.Game.Tests
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock; // Use the interface
        private readonly OrdersController _ordersController;

        public OrdersControllerTests()
        {
            _orderServiceMock = new Mock<IOrderService>(); // Mock the interface
            _ordersController = new OrdersController(_orderServiceMock.Object);
        }

        private void SetupUserClaims(string userId)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _ordersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task CreateOrder_WithAuthenticatedUser_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();
            var request = new CreateOrderRequest(new List<OrderItemRequest>());

            SetupUserClaims(userId.ToString());

            _orderServiceMock.Setup(s => s.CreateOrderAsync(userId, request.Items))
                .ReturnsAsync(expectedOrderId);

            // Act
            var result = await _ordersController.CreateOrder(request);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(OrdersController.GetOrder));
            
            createdAtActionResult.RouteValues.Should().NotBeNull();
            createdAtActionResult.RouteValues["id"].Should().Be(expectedOrderId);
            
            // Verify service was called with the correct UserId from claims
            _orderServiceMock.Verify(s => s.CreateOrderAsync(userId, request.Items), Times.Once);
        }
    }
}
