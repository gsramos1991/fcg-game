using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Repositories;
using FCG.Game.Application.Services;
using FCG.Game.Application.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FCG.Game.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IMessagePublisher> _messagePublisherMock;
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly IOrderService _orderService;

        public OrderServiceTests()
        {
            _messagePublisherMock = new Mock<IMessagePublisher>();
            _gameRepositoryMock = new Mock<IGameRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _gameRepositoryMock.Object,
                _messagePublisherMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldPublishMessageAndReturnOrderId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var gamePrice = 100m;
            var gameTitle = "Test Game";

            var orderItems = new List<OrderItemRequest>
            {
                new(gameId, 2)
            };

            var game = new Domain.Entities.Game(
                gameId,
                gameTitle,
                "Test Description",
                "Test Genre",
                gamePrice,
                "Test Publisher",
                DateTime.Now,
                new List<string>(),
                ""
            );

            _gameRepositoryMock.Setup(r => r.GetGameByIdAsync(gameId)).ReturnsAsync(game);

            string publishedMessage = null;
            _messagePublisherMock.Setup(p => p.Publish(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((msg, queue) => publishedMessage = msg);

            // Act
            var result = await _orderService.CreateOrderAsync(userId, orderItems);

            // Assert
            result.Should().NotBeEmpty();

            _messagePublisherMock.Verify(p => p.Publish(It.IsAny<string>(), "payment-requests"), Times.Once);
            
            publishedMessage.Should().NotBeNull();
            var publishedOrder = JsonSerializer.Deserialize<OrderApiRequest>(publishedMessage);
            publishedOrder.UserId.Should().Be(userId.ToString());
            publishedOrder.Items.Should().HaveCount(1);
            publishedOrder.Items[0].JogoId.Should().Be(gameId.ToString());

            _orderRepositoryMock.Verify(r => r.AddOrderAsync(It.Is<Domain.Entities.Order>(o => o.Id == result)), Times.Once);
        }
    }
}
