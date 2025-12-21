using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Repositories;
using FCG.Game.Application.Services;
using FCG.Game.Application.Services.Interfaces;
using Moq;

namespace FCG.Game.Tests
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly IGameService _gameService;

        public GameServiceTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _gameService = new GameService(_gameRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateGameAsync_ShouldCallRepositoryAndReturnId()
        {
            // Arrange
            var dto = new CreateGameDto(
                "New Game",
                "Description",
                "Genre",
                49.99m,
                "Publisher",
                DateTime.Now,
                new List<string>(),
                ""
            );

            // Act
            var gameId = await _gameService.CreateGameAsync(dto);

            // Assert
            Assert.NotEqual(Guid.Empty, gameId);
            _gameRepositoryMock.Verify(r => r.AddGameAsync(It.Is<Domain.Entities.Game>(g => 
                g.Id == gameId && g.Title == dto.Title
            )), Times.Once);
        }
    }
}