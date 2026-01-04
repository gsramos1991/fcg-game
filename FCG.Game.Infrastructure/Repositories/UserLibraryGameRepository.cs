using FCG.Game.Application.Repositories;
using FCG.Game.Domain.Entities;
using FCG.Game.Infrastructure.Data;
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
    }
}
