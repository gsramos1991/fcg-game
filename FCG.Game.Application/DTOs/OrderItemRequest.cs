using System;

namespace FCG.Game.Application.DTOs
{
    public record OrderItemRequest(Guid GameId, int Quantity);
}
