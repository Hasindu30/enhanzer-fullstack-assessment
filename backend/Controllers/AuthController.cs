using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var rawResponse = await _authService.LoginAsync(request);
            return Content(rawResponse, "application/json");
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
}
