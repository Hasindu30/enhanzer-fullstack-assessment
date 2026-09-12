using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly TokenService _tokenService;

    public AuthController(IAuthService authService, TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(result);

            var token = _tokenService.CreateToken(request.Email);

            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(result);
        }
        catch (ApplicationException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        return Ok(new { email });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token", new CookieOptions
        {
            SameSite = SameSiteMode.None,
            Secure = true
        });
        return Ok(new { message = "Logged out." });
    }
}

