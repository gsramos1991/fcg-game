using FCG.Game.Application.DTOs;

namespace FCG.Game.Application.Clients
{
    public interface IOrderApiClient
    {
        Task<Guid> CreateOrderAsync(OrderApiRequest orderRequest);
    }
}
