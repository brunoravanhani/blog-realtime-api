using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlogRealtime.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserApplication _userApplication;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserApplication userApplication, ITokenService tokenService, ILogger<AuthController> logger)
    {
        _userApplication = userApplication;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        _logger.LogInformation("AuthController - Login attempt for email: {Email}", dto.Email);
        try
        {
            var user = await _userApplication.ValidateLogin(dto) ?? throw new UnauthorizedAccessException("Invalid email or password.");
            var token = _tokenService.GenerateToken(user);
            var response = new AuthResponseDto(token, new UserDto(user.Id, user.Name, user.Email));

            _logger.LogInformation("AuthController - Login successful for email: {Email}", dto.Email);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "AuthController - Login failed for email: {Email}", dto.Email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthController - Unexpected error during login for email: {Email}", dto.Email);
            throw;
        }
    }
}

