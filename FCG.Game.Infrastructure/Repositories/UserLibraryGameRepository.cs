using FCG.Game.Application.Repositories;
using FCG.Game.Domain.Entities;
using FCG.Game.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Game.Infrastructure.Repositories
{
    public class UserLibraryGameRepository : IUserLibraryGameRepository
    {
        private readonly GameDbContext _context;
        public UserLibraryGameRepository(GameDbContext context)
        {
            _context = context;
        }
        public async Task addGameOnLibrary(UserLibraryGame game)
        {
            _context.UserLibraryGames.Add(game);
            await _context.SaveChangesAsync();

        }

        public async Task<List<UserLibraryGame>> findGameOnLibrary(Guid orderId, Guid userId)
        {
            return await _context.UserLibraryGames.Where(x => x.orderId == orderId && x.userId == userId)
                                                  .AsNoTracking()
                                                  .ToListAsync();
        }

        public async Task updateGameOnLibrary(UserLibraryGame libraryGuid)
        {

            _context.UserLibraryGames.Update(libraryGuid);
            await _context.SaveChangesAsync();


        }
    }
}
