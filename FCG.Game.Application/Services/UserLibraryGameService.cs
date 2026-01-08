using FCG.Game.Application.DTOs;
using FCG.Game.Application.Repositories;
using FCG.Game.Application.Services.Interfaces;
using FCG.Game.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace FCG.Game.Application.Services
{
    public class UserLibraryGameService : IUserLibraryGameService
    {
        private readonly IUserLibraryGameRepository _userLibraryGameRepository;
        public UserLibraryGameService(IUserLibraryGameRepository userLibraryGameRepository)
        {
            _userLibraryGameRepository = userLibraryGameRepository;
        }
        public async Task InsertGameUser(CreateOrderRequest createOrderRequest, Guid userId, Guid orderId)
        {
            foreach (var item in createOrderRequest.Items)
            {
                var userLibraryGame = new Domain.Entities.UserLibraryGame()
                {
                    idLibraryGame = new Guid(),
                    orderId = orderId,
                    userId = userId,
                    idGame = item.GameId,
                    isActive = false,
                    createdAt = DateTime.Now
                };

                await _userLibraryGameRepository.addGameOnLibrary(userLibraryGame);
            }
        }

        public async Task UpdateGameUser(Order order, List<UserLibraryGame> library)
        {
            foreach (var item in library)
            {   
                if (!InvalidStatus(order))
                {
                    continue;
                }

                item.isActive = true;
                await _userLibraryGameRepository.updateGameOnLibrary(item);

            }
        }

        private static bool InvalidStatus(Order order)
        {
            var validStatuses = new[] {
                OrderStatus.PENDING,
                OrderStatus.ERROR,
                OrderStatus.FAIL,
                OrderStatus.CANCELLED
            };

            return !validStatuses.Contains(order.Status);
        }

        public async Task<List<UserLibraryGame>> FindGameUser(Guid orderId , Guid userId)
        {
            return await _userLibraryGameRepository.findGameOnLibrary(orderId, userId);
        }
    }
}
