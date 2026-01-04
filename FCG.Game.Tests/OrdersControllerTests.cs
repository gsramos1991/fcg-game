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
using FCG.Game.Domain.Entities;
using System.Linq;

namespace FCG.Game.Tests
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock; // Use the interface
        private readonly Mock<IUserLibraryGameService> _userLibraryGameServiceMock;
        private readonly OrdersController _ordersController;

        public OrdersControllerTests()
        {
            _orderServiceMock = new Mock<IOrderService>(); // Mock the interface
            _userLibraryGameServiceMock = new Mock<IUserLibraryGameService>();
            _ordersController = new OrdersController(_orderServiceMock.Object, _userLibraryGameServiceMock.Object);
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

        [Fact]
        public async Task CompleteOrder_ReturnsOk_WhenServiceReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            SetupUserClaims(userId.ToString());

            _orderServiceMock.Setup(s => s.CompleteOrderAsync(orderId, userId)).ReturnsAsync(true);

            // Act
            var result = await _ordersController.CompleteOrder(orderId);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var val = ok.Value!;
            var msgProp = val.GetType().GetProperty("message");
            msgProp.Should().NotBeNull();
            ((string)msgProp.GetValue(val)!).Should().Contain("com sucesso");
        }

        [Fact]
        public async Task CompleteOrder_ReturnsNotFound_WhenServiceReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            SetupUserClaims(userId.ToString());

            _orderServiceMock.Setup(s => s.CompleteOrderAsync(orderId, userId)).ReturnsAsync(false);

            // Act
            var result = await _ordersController.CompleteOrder(orderId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CompleteOrder_ReturnsBadRequest_WhenServiceThrowsInvalidOperation()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            SetupUserClaims(userId.ToString());

            _orderServiceMock.Setup(s => s.CompleteOrderAsync(orderId, userId)).ThrowsAsync(new InvalidOperationException("already processed"));

            // Act
            var result = await _ordersController.CompleteOrder(orderId);

            // Assert
            var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var val = bad.Value!;
            var errProp = val.GetType().GetProperty("error");
            errProp.Should().NotBeNull();
            ((string)errProp.GetValue(val)!).Should().Contain("already processed");
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderServiceMock.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync((Order?)null);

            // Act
            var result = await _ordersController.GetOrder(orderId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetOrder_ReturnsOk_WithOrderDto_WhenServiceReturnsOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                UserId = Guid.NewGuid(),
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 100m
            };

            _orderServiceMock.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _ordersController.GetOrder(orderId);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeOfType<OrderDto>().Subject;
            dto.Id.Should().Be(orderId);
        }

        [Fact]
        public async Task GetMyOrders_ReturnsPagedResult_WithExpectedShape()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupUserClaims(userId.ToString());
            var page = 1;
            var pageSize = 10;
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Pending, CreatedAt = DateTime.UtcNow }
            };

            _orderServiceMock.Setup(s => s.GetUserOrdersAsync(userId, page, pageSize)).ReturnsAsync(orders);

            // Act
            var result = await _ordersController.GetMyOrders(page, pageSize);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = ok.Value!;
            var pageProp = value.GetType().GetProperty("page");
            pageProp.Should().NotBeNull();
            ((int)pageProp.GetValue(value)!).Should().Be(page);

            var dataProp = value.GetType().GetProperty("data");
            dataProp.Should().NotBeNull();
            var dataVal = (IEnumerable<object>)dataProp.GetValue(value)!;
            dataVal.Count().Should().Be(orders.Count);
        }
    }
}
