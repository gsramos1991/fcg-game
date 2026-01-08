using FCG.Game.Application.DTOs;
using FCG.Game.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.Game.API.Controllers;

[Authorize(Roles = "Usuario,Administrador")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IUserLibraryGameService _userLibraryGame;
    public OrdersController(IOrderService orderService, IUserLibraryGameService userLibraryGame)
    {
        _orderService = orderService;
        _userLibraryGame = userLibraryGame;
    }

    [HttpPost("new-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var orderId = await _orderService.CreateOrderAsync(userId, request.Items, request);
          
            return CreatedAtAction(null, new { id = orderId }, orderId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteOrder(Guid id)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var success = await _orderService.CompleteOrderAsync(id, userId);

            if (!success)
                return NotFound();

            return Ok(new { message = "Pedido completado com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = User.FindFirst("jti")?.Value;
        }

        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

