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
        private readonly IUserLibraryGameService _library;

        public OrderService(
            IOrderRepository orderRepository,
            IGameRepository gameRepository,
            IMessagePublisher messagePublisher,
            IUserLibraryGameService library)
        {
            _orderRepository = orderRepository;
            _gameRepository = gameRepository;
            _messagePublisher = messagePublisher;
            _library = library;
        }

        public async Task<Guid> CreateOrderAsync(Guid userId, List<OrderItemRequest> items, CreateOrderRequest request)
        {
            var gameIds = items.Select(i => i.GameId).ToList();
            var games = new List<Domain.Entities.Game>();
            var userLibrary = new List<Domain.Entities.UserLibraryGame>();
            var orderId = Guid.NewGuid();
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


            await _library.InsertGameUser(request, userId, orderId);


            var orderApiRequest = new OrderApiRequest
            {
                OrderId = orderId,
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


            var order = MontarPedido(Guid.Empty, orderApiRequest);
            await _orderRepository.AddOrderAsync(order);

            var jsonPayload = JsonSerializer.Serialize(orderApiRequest);
            await _messagePublisher.Publish(jsonPayload, "payment-requests");

            
            return order.Id;
        }

        public async Task<bool> CompleteOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null || order.UserId != userId)
                return false;

            if (order.Status != OrderStatus.PENDING)
                throw new InvalidOperationException("Order has already been processed.");

            order.Complete();
            await _orderRepository.UpdateOrderAsync(order);
          
            return true;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId, PaymentResponseDto resp)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (Enum.TryParse(resp.StatusPayment, out OrderStatus meuStatus))
            {
                order.Status = meuStatus;
                order.CompletedAt = DateTime.UtcNow;

                await _orderRepository.UpdateOrderAsync(order);
            }

            return order;
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
                Status = OrderStatus.PENDING,
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