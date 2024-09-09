using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Responses;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    // Constructor that injects the IAuthService dependency.
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    // Registers a new user. Returns the user's ID if successful, or a bad request if an error occurs.
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
    {
        try
        {
            var userId = await _authService.RegisterAsync(registerDto);
            return Created($"/api/auth/{userId}", userId); // Returns 201 Created with user ID.
        }
        catch (AuthenticationException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e); // Returns 400 Bad Request with the error message.
        }
    }
    
    // Authenticates a user. Returns a JWT token if successful, or a bad request if authentication fails.
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginDto)
    {
        try
        {
            var token = await _authService.LoginAsync(loginDto);
            return Ok(token); // Returns 200 OK with the JWT token.
        }
        catch (AuthenticationException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e); // Returns 400 Bad Request with the error message.
        }
    }
}
