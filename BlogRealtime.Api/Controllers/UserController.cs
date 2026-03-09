using Microsoft.AspNetCore.Mvc;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;

namespace BlogRealtime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public record CreateUserDto(string Name, string Email, string Password);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var user = new User(dto.Name, dto.Email, dto.Password);
        await _userService.Add(user);
        return Created();
    }
}
