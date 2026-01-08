using System;
using System.Collections.Generic;

namespace FCG.Game.Application.DTOs
{
    public class PaymentRequestDto
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string? StatusPayment { get; set; }
    }

    public class PaymentResponseDto
    {
        public Guid orderId { get; set; } = Guid.Empty;
        public Guid PaymentId { get; set; }
        public string? StatusPayment { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
