using System.Collections.Generic;

namespace FCG.Game.Application.DTOs
{
    public record CreateOrderRequest(List<OrderItemRequest> Items);
}
