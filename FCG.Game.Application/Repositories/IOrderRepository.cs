using FCG.Game.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FCG.Game.Application.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync(Guid userId, int page, int pageSize);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Guid id);
    }
}
