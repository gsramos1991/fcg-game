using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FCG.Game.Application.Clients;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Repositories;
using FCG.Game.Application.Services;
using FCG.Game.Application.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace FCG.Game.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderApiClient> _orderApiClientMock;
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly IOrderService _orderService;

        public OrderServiceTests()
        {
            _orderApiClientMock = new Mock<IOrderApiClient>();
            _gameRepositoryMock = new Mock<IGameRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _gameRepositoryMock.Object,
                _orderApiClientMock.Object);
        }

        //[Fact]
        //public async Task CreateOrderAsync_ShouldCallApiClientAndReturnOrderId()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var gameId = Guid.NewGuid();
        //    var expectedOrderId = Guid.NewGuid();
        //    var gamePrice = 100m;
        //    var gameTitle = "Test Game";

        //    var orderItems = new List<OrderItemRequest>
        //    {
        //        new(gameId, 2)
        //    };

        //    var game = new Domain.Entities.Game(
        //        gameId, 
        //        gameTitle, 
        //        "Test Description", 
        //        "Test Genre", 
        //        gamePrice,
        //        "Test Publisher",
        //        DateTime.Now,
        //        new List<string>(),
        //        ""
        //    );

        //    _gameRepositoryMock.Setup(r => r.GetGameByIdAsync(gameId)).ReturnsAsync(game);

        //    _orderApiClientMock.Setup(c => c.CreateOrderAsync(It.IsAny<OrderApiRequest>()))
        //        .ReturnsAsync(expectedOrderId);

        //    // Act
        //    var result = await _orderService.CreateOrderAsync(userId, orderItems);

        //    // Assert
        //    result.Should().Be(expectedOrderId);

        //    _orderApiClientMock.Verify(c => c.CreateOrderAsync(It.Is<OrderApiRequest>(req =>
        //        req.UserId == userId.ToString() &&
        //        req.Currency == "BRL" &&
        //        req.Items.Count == 1 &&
        //        req.Items[0].JogoId == gameId.ToString() &&
        //        req.Items[0].Description == gameTitle &&
        //        req.Items[0].UnitPrice == gamePrice &&
        //        req.Items[0].Quantity == 2
        //    )), Times.Once);

        //    _orderRepositoryMock.Verify(r => r.AddOrderAsync(It.IsAny<Domain.Entities.Order>()), Times.Never);
        //}
    }
}
