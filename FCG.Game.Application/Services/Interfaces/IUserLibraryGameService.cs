using FCG.Game.Application.DTOs;
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
    }
}
