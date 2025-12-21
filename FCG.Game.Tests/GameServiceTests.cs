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

        [Fact]
        public async Task GetGameByIdAsync_ReturnsGame_WhenRepositoryReturnsGame()
        {
            // Arrange
            var id = Guid.NewGuid();
            var game = new FCG.Game.Domain.Entities.Game(id, "T", "D", "G", 10m, "P", DateTime.UtcNow, new List<string>(), "");
            _gameRepositoryMock.Setup(r => r.GetGameByIdAsync(id)).ReturnsAsync(game);

            // Act
            var result = await _gameService.GetGameByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
        }

        [Fact]
        public async Task SearchGamesAsync_DelegatesToRepository()
        {
            // Arrange
            var term = "foo";
            var page = 1;
            var pageSize = 10;
            var games = new List<FCG.Game.Domain.Entities.Game>
            {
                new FCG.Game.Domain.Entities.Game(Guid.NewGuid(), "A", "D", "G", 1m, "P", DateTime.UtcNow, new List<string>(), "")
            };

            _gameRepositoryMock.Setup(r => r.SearchGamesAsync(term, page, pageSize)).ReturnsAsync(games);

            // Act
            var result = await _gameService.SearchGamesAsync(term, page, pageSize);

            // Assert
            Assert.Same(games, result);
            _gameRepositoryMock.Verify(r => r.SearchGamesAsync(term, page, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetGamesByGenreAsync_DelegatesToRepository()
        {
            // Arrange
            var genre = "Action";
            var limit = 5;
            var games = new List<FCG.Game.Domain.Entities.Game>();
            _gameRepositoryMock.Setup(r => r.GetGamesByGenreAsync(genre, limit)).ReturnsAsync(games);

            // Act
            var result = await _gameService.GetGamesByGenreAsync(genre, limit);

            // Assert
            Assert.Same(games, result);
            _gameRepositoryMock.Verify(r => r.GetGamesByGenreAsync(genre, limit), Times.Once);
        }

        [Fact]
        public async Task GetRecommendationsAsync_UsesMostPopular_WhenCalled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var limit = 3;
            var games = new List<FCG.Game.Domain.Entities.Game>
            {
                new FCG.Game.Domain.Entities.Game(Guid.NewGuid(), "A", "D", "G", 1m, "P", DateTime.UtcNow, new List<string>(), "")
            };

            _gameRepositoryMock.Setup(r => r.GetMostPopularGamesAsync(limit)).ReturnsAsync(games);

            // Act
            var result = await _gameService.GetRecommendationsAsync(userId, limit);

            // Assert
            Assert.Same(games, result);
            _gameRepositoryMock.Verify(r => r.GetMostPopularGamesAsync(limit), Times.Once);
        }

        [Fact]
        public async Task GetMostPopularGamesAsync_DelegatesToRepository()
        {
            // Arrange
            var limit = 7;
            var games = new List<FCG.Game.Domain.Entities.Game>();
            _gameRepositoryMock.Setup(r => r.GetMostPopularGamesAsync(limit)).ReturnsAsync(games);

            // Act
            var result = await _gameService.GetMostPopularGamesAsync(limit);

            // Assert
            Assert.Same(games, result);
            _gameRepositoryMock.Verify(r => r.GetMostPopularGamesAsync(limit), Times.Once);
        }
    }
}