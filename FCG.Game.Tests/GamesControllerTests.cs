using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FCG.Game.API.Controllers;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Services.Interfaces; // Add this using
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
    }
}