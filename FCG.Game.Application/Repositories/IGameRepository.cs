using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FCG.Game.Application.Repositories
{
    public interface IGameRepository
    {
        Task<Domain.Entities.Game> GetGameByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Game>> GetAllGamesAsync();
        Task<List<Domain.Entities.Game>> SearchGamesAsync(string searchTerm, int page, int pageSize);
        Task<List<Domain.Entities.Game>> GetGamesByGenreAsync(string genre, int limit);
        Task<List<Domain.Entities.Game>> GetMostPopularGamesAsync(int limit);
        Task AddGameAsync(Domain.Entities.Game game);
        Task UpdateGameAsync(Domain.Entities.Game game);
        Task DeleteGameAsync(Guid id);
    }
}