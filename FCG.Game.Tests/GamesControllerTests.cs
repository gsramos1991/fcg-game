using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FCG.Game.API.Controllers;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Services.Interfaces; // Add this using
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FCG.Game.Tests
{
    public class GamesControllerTests
    {
        private readonly Mock<IGameService> _gameServiceMock; // Use the interface
        private readonly GamesController _gamesController;

        public GamesControllerTests()
        {
            _gameServiceMock = new Mock<IGameService>(); // Mock the interface
            _gamesController = new GamesController(_gameServiceMock.Object);
        }

        private void SetupUserClaims(string userId, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _gamesController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task CreateGame_WithAdminUser_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var adminUserId = Guid.NewGuid();
            var expectedGameId = Guid.NewGuid();
            var dto = new CreateGameDto(
                "Test Game",
                "A game for testing",
                "Action",
                59.99m,
                "Test Publisher",
                DateTime.Now,
                new List<string> { "test", "action" },
                "http://example.com/cover.jpg"
            );

            SetupUserClaims(adminUserId.ToString(), "ADMIN");

            _gameServiceMock.Setup(s => s.CreateGameAsync(dto))
                .ReturnsAsync(expectedGameId);

            // Act
            var result = await _gamesController.CreateGame(dto);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(GamesController.GetGame));
            
            createdAtActionResult.RouteValues.Should().NotBeNull();
            createdAtActionResult.RouteValues["id"].Should().Be(expectedGameId);
            
            // Verify service was called
            _gameServiceMock.Verify(s => s.CreateGameAsync(dto), Times.Once);
        }

        [Fact]
        public async Task GetGame_ReturnsNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _gameServiceMock.Setup(s => s.GetGameByIdAsync(id)).ReturnsAsync((FCG.Game.Domain.Entities.Game?)null);

            // Act
            var result = await _gamesController.GetGame(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetGame_ReturnsOk_WithGameDto_WhenServiceReturnsGame()
        {
            // Arrange
            var id = Guid.NewGuid();
            var game = new FCG.Game.Domain.Entities.Game(id, "Title", "Desc", "Genre", 9.99m, "Pub", DateTime.UtcNow, new List<string>(), "");
            _gameServiceMock.Setup(s => s.GetGameByIdAsync(id)).ReturnsAsync(game);

            // Act
            var result = await _gamesController.GetGame(id);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeOfType<GameDto>().Subject;
            dto.Id.Should().Be(id);
            dto.Title.Should().Be("Title");
        }

        [Fact]
        public async Task SearchGames_ReturnsPagedResult_WithExpectedShape()
        {
            // Arrange
            var term = "test";
            var page = 2;
            var pageSize = 5;
            var games = new List<FCG.Game.Domain.Entities.Game>
            {
                new FCG.Game.Domain.Entities.Game(Guid.NewGuid(), "A", "D", "G", 1m, "P", DateTime.UtcNow, new List<string>(), ""),
                new FCG.Game.Domain.Entities.Game(Guid.NewGuid(), "B", "D", "G", 2m, "P", DateTime.UtcNow, new List<string>(), "")
            };

            _gameServiceMock.Setup(s => s.SearchGamesAsync(term, page, pageSize)).ReturnsAsync(games);

            // Act
            var result = await _gamesController.SearchGames(term, page, pageSize);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = ok.Value!;
            var pageProp = value.GetType().GetProperty("page");
            pageProp.Should().NotBeNull();
            ((int)pageProp.GetValue(value)!).Should().Be(page);

            var pageSizeProp = value.GetType().GetProperty("pageSize");
            pageSizeProp.Should().NotBeNull();
            ((int)pageSizeProp.GetValue(value)!).Should().Be(pageSize);

            var totalProp = value.GetType().GetProperty("total");
            totalProp.Should().NotBeNull();
            ((int)totalProp.GetValue(value)!).Should().Be(games.Count);

            var dataProp = value.GetType().GetProperty("data");
            dataProp.Should().NotBeNull();
            var dataVal = (IEnumerable<object>)dataProp.GetValue(value)!;
            dataVal.Count().Should().Be(games.Count);
        }

        [Fact]
        public async Task GetRecommendations_ReturnsOk_WithRecommendationsAndUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var limit = 3;
            SetupUserClaims(userId.ToString(), "Usuario");

            var games = new List<FCG.Game.Domain.Entities.Game>
            {
                new FCG.Game.Domain.Entities.Game(Guid.NewGuid(), "A", "D", "G", 1m, "P", DateTime.UtcNow, new List<string>(), "")
            };

            _gameServiceMock.Setup(s => s.GetRecommendationsAsync(userId, limit)).ReturnsAsync(games);

            // Act
            var result = await _gamesController.GetRecommendations(limit);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = ok.Value!;
            var userIdProp = value.GetType().GetProperty("userId");
            userIdProp.Should().NotBeNull();
            ((Guid)userIdProp.GetValue(value)!).Should().Be(userId);

            var countProp = value.GetType().GetProperty("count");
            countProp.Should().NotBeNull();
            ((int)countProp.GetValue(value)!).Should().Be(games.Count);

            var recProp = value.GetType().GetProperty("recommendations");
            recProp.Should().NotBeNull();
            ((IEnumerable<object>)recProp.GetValue(value)!).Should().NotBeNull();
        }
    }
}