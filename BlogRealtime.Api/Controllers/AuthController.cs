using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogRealtime.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _userService.ValidateLogin(dto);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _tokenService.GenerateToken(user);
        var response = new AuthResponseDto(token, new UserDto(user.Id, user.Name, user.Email));

        return Ok(response);
    }
}

