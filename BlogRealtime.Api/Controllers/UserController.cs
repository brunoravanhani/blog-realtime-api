using Microsoft.AspNetCore.Mvc;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlogRealtime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserApplication _userApplication;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserApplication userApplication, ILogger<UserController> logger)
    {
        _userApplication = userApplication;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        _logger.LogInformation("UserController - Creating user with email: {Email}", dto.Email);
        try
        {
            await _userApplication.Create(dto);
            _logger.LogInformation("UserController - User created successfully. Email: {Email}", dto.Email);
            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UserController - Error creating user with email: {Email}", dto.Email);
            throw;
        }
    }
}
