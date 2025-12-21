using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FCG.Game.Application.DTOs;

namespace FCG.Game.Application.Services.Interfaces
{
    public interface IGameService
    {
        Task<Guid> CreateGameAsync(CreateGameDto dto);
        Task<Domain.Entities.Game?> GetGameByIdAsync(Guid gameId);
        Task<List<Domain.Entities.Game>> SearchGamesAsync(string searchTerm, int page = 1, int pageSize = 20);
        Task<List<Domain.Entities.Game>> GetGamesByGenreAsync(string genre, int limit = 20);
        Task<List<Domain.Entities.Game>> GetRecommendationsAsync(Guid userId, int limit = 10);
        Task<List<Domain.Entities.Game>> GetMostPopularGamesAsync(int limit = 10);
    }
}
