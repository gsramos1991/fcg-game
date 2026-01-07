using FCG.Game.Application.DTOs;
using FCG.Game.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Game.Application.Services.Interfaces
{
    public interface IUserLibraryGameService
    {
        Task InsertGameUser(CreateOrderRequest createOrderRequest, Guid userId, Guid orderId);

        Task UpdateGameUser(Order order, List<UserLibraryGame> library);
        Task<List<UserLibraryGame>> FindGameUser(Guid orderId, Guid userId);
    }
}
