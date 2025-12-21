using System;
using System.Collections.Generic;
using System.Linq;
using FCG.Game.Domain.Entities;

namespace FCG.Game.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? PaymentId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public static OrderDto? FromOrder(Order order)
        {
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                PaymentId = order.PaymentId ?? Guid.Empty,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt
            };
        }
    }

    public class OrderItemDto
    {
        public Guid GameId { get; set; }
        public string GameTitle { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public static OrderItemDto FromOrderItem(OrderItem item)
        {
            return new OrderItemDto
            {
                GameId = item.GameId,
                GameTitle = item.GameTitle,
                Price = item.Price,
                Quantity = item.Quantity
            };
        }
    }
}