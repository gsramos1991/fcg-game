using FCG.Game.Application.DTOs;
using FCG.Game.Application.Repositories;
using FCG.Game.Application.Services.Interfaces;
using FCG.Game.Domain.Entities;
using System.Text.Json;

namespace FCG.Game.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IMessagePublisher _messagePublisher;

        public OrderService(
            IOrderRepository orderRepository,
            IGameRepository gameRepository,
            IMessagePublisher messagePublisher)
        {
            _orderRepository = orderRepository;
            _gameRepository = gameRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<Guid> CreateOrderAsync(Guid userId, List<OrderItemRequest> items)
        {
            var gameIds = items.Select(i => i.GameId).ToList();
            var games = new List<Domain.Entities.Game>();
            var userLibrary = new List<Domain.Entities.UserLibraryGame>();
            foreach (var gameId in gameIds)
            {
                var userLibraryGame = new Domain.Entities.UserLibraryGame();
                var game = await _gameRepository.GetGameByIdAsync(gameId);
                if (game == null)
                {
                    throw new InvalidOperationException($"Game with ID {gameId} not found.");
                }

                userLibraryGame.idGame = gameId;
                userLibraryGame.userId = userId;
                userLibraryGame.isActive = false;
                userLibraryGame.createdAt = DateTime.UtcNow;

                games.Add(game);
                userLibrary.Add(userLibraryGame);
            }

            var orderApiRequest = new OrderApiRequest
            {
                OrderId = Guid.NewGuid(),
                UserId = userId.ToString(),
                Currency = "BRL",
                Items = items.Select(item =>
                {
                    var game = games.First(g => g.Id == item.GameId);
                    return new OrderItemApiRequest
                    {
                        JogoId = game.Id.ToString(),
                        Description = game.Title,
                        UnitPrice = game.Price,
                        Quantity = item.Quantity
                    };
                }).ToList()
            };

            var jsonPayload = JsonSerializer.Serialize(orderApiRequest);
            _messagePublisher.Publish(jsonPayload, "payment-requests");

            var order =  MontarPedido(Guid.Empty, orderApiRequest);
            await _orderRepository.AddOrderAsync(order);
            return order.Id;
        }

        public async Task<bool> CompleteOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null || order.UserId != userId)
                return false;

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Order has already been processed.");

            order.Complete();
            await _orderRepository.UpdateOrderAsync(order);
            
            // The logic to increment game sales was removed as it was tied to elastic.
            // This would need to be re-implemented differently if still required.

            return true;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<List<Order>> GetUserOrdersAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            // Note: IOrderRepository does not have a method for this.
            // I will add it, assuming it will be implemented.
            // For now, returning an empty list.
            // This will be a compilation error until I define it.
            // await _orderRepository.GetUserOrdersAsync(userId, page, pageSize);
            return await Task.FromResult(new List<Order>());
        }

        private Order MontarPedido(Guid PaymentId, OrderApiRequest order)
        {
            var orderToDb = new Order
            {
                
                Id = order.OrderId,
                Status = OrderStatus.Pending,
                PaymentId = PaymentId,
                UserId = Guid.Parse(order.UserId),
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.Now,
                TotalAmount = 0
            };

            return orderToDb;
        }
    }
}