using System;
using System.Threading.Tasks;
using FCG.Game.Application.DTOs;

namespace FCG.Game.Application.Clients
{
    public interface IPaymentApiClient
    {
        Task<PaymentResponseDto?> ConsultPaymentAsync(Guid paymentId, Guid userId);
        Task<PaymentResponseDto?> CancelPaymentAsync(PaymentRequestDto request);
    }
}
