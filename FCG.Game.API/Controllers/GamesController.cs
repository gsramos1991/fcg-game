using FCG.Game.Application.Services.Interfaces;
using FCG.Game.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Game.API.Controllers;

[Authorize]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameDto dto)
    {
        var gameId = await _gameService.CreateGameAsync(dto);
        return CreatedAtAction(nameof(GetGame), new { id = gameId }, new { gameId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGame(Guid id)
    {
        var game = await _gameService.GetGameByIdAsync(id);

        if (game == null)
            return NotFound();

        return Ok(GameDto.FromGame(game));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchGames(
        [FromQuery] string term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var games = await _gameService.SearchGamesAsync(term, page, pageSize);
        var gameDtos = games.Select(GameDto.FromGame).ToList();

        return Ok(new
        {
            page,
            pageSize,
            total = gameDtos.Count,
            data = gameDtos
        });
    }

    [HttpGet("genre/{genre}")]
    public async Task<IActionResult> GetByGenre(string genre, [FromQuery] int limit = 20)
    {
        var games = await _gameService.GetGamesByGenreAsync(genre, limit);
        return Ok(games.Select(GameDto.FromGame));
    }

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopular([FromQuery] int limit = 10)
    {
        var games = await _gameService.GetMostPopularGamesAsync(limit);
        return Ok(games.Select(GameDto.FromGame));
    }
}