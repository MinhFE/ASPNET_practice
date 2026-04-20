using DotnetApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(string email, string roles)
  {
    var result = await _authService.LoginAsync(email, roles);
    return Ok(result);
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] string refreshToken, [FromQuery] string email, [FromQuery] string roles)
  {
    var result = await _authService.RefreshToken(refreshToken, email, roles);
    return Ok(result);
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] string refreshToken)
  {
    await _authService.Logout(refreshToken);
    return Ok();
  }
}