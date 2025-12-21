using FCG.Game.Application.DTOs;
using FCG.Game.Application.Repositories;
using FCG.Game.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FCG.Game.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<Guid> CreateGameAsync(CreateGameDto dto)
        {
            var game = new Domain.Entities.Game(
                Guid.NewGuid(),
                dto.Title,
                dto.Description,
                dto.Genre,
                dto.Price,
                dto.Publisher,
                dto.ReleaseDate,
                dto.Tags,
                dto.CoverImageUrl
            );

            await _gameRepository.AddGameAsync(game);
            return game.Id;
        }

        public async Task<Domain.Entities.Game?> GetGameByIdAsync(Guid gameId)
        {
            return await _gameRepository.GetGameByIdAsync(gameId);
        }

        public async Task<List<Domain.Entities.Game>> SearchGamesAsync(string searchTerm, int page = 1, int pageSize = 20)
        {
            return await _gameRepository.SearchGamesAsync(searchTerm, page, pageSize);
        }

        public async Task<List<Domain.Entities.Game>> GetGamesByGenreAsync(string genre, int limit = 20)
        {
            return await _gameRepository.GetGamesByGenreAsync(genre, limit);
        }

        public async Task<List<Domain.Entities.Game>> GetRecommendationsAsync(Guid userId, int limit = 10)
        {
            // Simplified recommendation: return most popular games.
            return await GetMostPopularGamesAsync(limit);
        }

        public async Task<List<Domain.Entities.Game>> GetMostPopularGamesAsync(int limit = 10)
        {
            return await _gameRepository.GetMostPopularGamesAsync(limit);
        }
    }
}