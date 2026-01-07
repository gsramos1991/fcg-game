using System;
using System.Threading.Tasks;
using FCG.Game.Application.Clients;
using FCG.Game.Application.DTOs;
using FCG.Game.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Game.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Usuario,Administrador")]
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentApiClient _paymentApiClient;
        private readonly IOrderService _orders;
        private readonly IUserLibraryGameService _library;

        public PaymentsController(IPaymentApiClient paymentApiClient, IOrderService order, IUserLibraryGameService library)
        {
            _paymentApiClient = paymentApiClient;
            _orders = order;
            _library = library;
        }

        [HttpGet("consultarPagamento")]
        public async Task<IActionResult> ConsultarPagamento(Guid paymentId, Guid userId)
        {
            var resp = await _paymentApiClient.ConsultPaymentAsync(paymentId, userId);
            if (resp == null)
                return NotFound();

            var order = await _orders.GetOrderByIdAsync(resp.orderId, resp);
            var jogos = await _library.FindGameUser(resp.orderId, userId);
            await _library.UpdateGameUser(order, jogos);

            if(jogos == null)
                return NotFound();



            return Ok(resp);
        }

        [HttpPost("cancelar")]
        public async Task<IActionResult> CancelarPagamento([FromBody] PaymentRequestDto request)
        {
            try
            {
                var resp = await _paymentApiClient.CancelPaymentAsync(request);
                if (resp == null)
                    return BadRequest(new { error = "Falha ao cancelar pagamento" });

                return Ok(resp);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
