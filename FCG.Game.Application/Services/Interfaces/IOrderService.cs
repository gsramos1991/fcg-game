using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FCG.Game.Application.DTOs;
using FCG.Game.Domain.Entities; // Add this using

namespace FCG.Game.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateOrderAsync(Guid userId, List<OrderItemRequest> items);
        Task<bool> CompleteOrderAsync(Guid orderId, Guid userId);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetUserOrdersAsync(Guid userId, int page = 1, int pageSize = 20);
    }
}