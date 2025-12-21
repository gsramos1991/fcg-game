using System;
using System.Threading.Tasks;
using FCG.Game.Application.Clients;
using FCG.Game.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Game.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentApiClient _paymentApiClient;

        public PaymentsController(IPaymentApiClient paymentApiClient)
        {
            _paymentApiClient = paymentApiClient;
        }

        [HttpGet("consultar/{paymentId}/{userId}")]
        public async Task<IActionResult> ConsultarPagamento(Guid paymentId, Guid userId)
        {
            var resp = await _paymentApiClient.ConsultPaymentAsync(paymentId, userId);
            if (resp == null)
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
