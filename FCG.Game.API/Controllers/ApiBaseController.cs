using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.Game.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiBaseController : ControllerBase
{
    /// <summary>
    /// Extrai o ID do usuario do token JWT.
    /// CORRECAO: Verifica ClaimTypes.NameIdentifier (padrao) e "sub" (comum em JWTs).
    /// </summary>
    protected Guid GetUserId()
    {
        // Prioriza o ClaimTypes.NameIdentifier, que e o padrao para ID de usuario
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Se nao encontrar, tenta o "sub", que e comum em muitos JWTs
        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = User.FindFirst("sub")?.Value;
        }

        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Extrai o token JWT do header Authorization
    /// </summary>
    protected string GetUserToken()
    {
        return Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    }

    /// <summary>
    /// Extrai o nome do usuario do token JWT
    /// </summary>
    protected string GetUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
    }

    /// <summary>
    /// Extrai o email do usuario do token JWT
    /// </summary>
    protected string GetUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }
}