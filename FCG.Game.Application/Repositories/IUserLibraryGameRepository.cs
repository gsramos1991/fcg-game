using FCG.Game.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Game.Application.Repositories
{
    public interface IUserLibraryGameRepository
    {
        Task addGameOnLibrary(UserLibraryGame game);
        Task updateGameOnLibrary(UserLibraryGame libraryGuid);
        Task<List<UserLibraryGame>> findGameOnLibrary(Guid orderId, Guid userId);
    }
}
