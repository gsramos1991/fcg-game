using FCG.Game.Application.DTOs;
using FCG.Game.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.Game.API.Controllers;

[Authorize(Roles = "Usuario,ADMIN")]
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
            var orderId = await _orderService.CreateOrderAsync(userId, request.Items);
            await _userLibraryGame.InsertGameUser(request, userId, orderId);
            return CreatedAtAction(nameof(GetOrder), new { id = orderId }, new { orderId });
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
            return NotFound();

        return Ok(OrderDto.FromOrder(order));
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetUserIdFromClaims();
        var orders = await _orderService.GetUserOrdersAsync(userId, page, pageSize);
        var orderDtos = orders.Select(OrderDto.FromOrder).ToList();

        return Ok(new
        {
            page,
            pageSize,
            total = orderDtos.Count,
            data = orderDtos
        });
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

