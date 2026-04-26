using CameraShop.API.DTOs.Auth;
using CameraShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CameraShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto) =>
        Ok(await _auth.RegisterAsync(dto));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto) =>
        Ok(await _auth.LoginAsync(dto));
}
