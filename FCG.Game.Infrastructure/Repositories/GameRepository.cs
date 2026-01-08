using FCG.Game.Application.Repositories;
using FCG.Game.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Game.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameDbContext _context;

        public GameRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Game> GetGameByIdAsync(Guid id)
        {
            try
            {
                return await _context.Games.FindAsync(id);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task<IEnumerable<Domain.Entities.Game>> GetAllGamesAsync()
        {
            return await _context.Games.ToListAsync();
        }

        public async Task AddGameAsync(Domain.Entities.Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGameAsync(Domain.Entities.Game game)
        {
            _context.Entry(game).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGameAsync(Guid id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Domain.Entities.Game>> SearchGamesAsync(string searchTerm, int page, int pageSize)
        {
            return await _context.Games
                .Where(g => g.Title.Contains(searchTerm) || g.Description.Contains(searchTerm))
                .OrderBy(g => g.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Game>> GetGamesByGenreAsync(string genre, int limit)
        {
            return await _context.Games
                .Where(g => g.Genre == genre)
                .OrderBy(g => g.Title)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Game>> GetMostPopularGamesAsync(int limit)
        {
            // This is a placeholder. "Popularity" is not defined in the SQL database.
            // Returning games ordered by Title for now.
            return await _context.Games
                .OrderBy(g => g.Title)
                .Take(limit)
                .ToListAsync();
        }
    }
}
