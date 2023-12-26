using GatewayService.AuthService;
using GatewayService.Server.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Server.Controllers;

/// <summary>
/// Контроллер для авторизации.
/// </summary>
[ApiController]
[Route("/oauth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("token")]
    public IActionResult Login([FromBody] LoginCredential loginCredential)
    {
        var token = _authService.Login(Guid.NewGuid(), loginCredential.Username, new List<string>(), string.Empty);
        
        return Ok(new AuthData(token));
    }
}