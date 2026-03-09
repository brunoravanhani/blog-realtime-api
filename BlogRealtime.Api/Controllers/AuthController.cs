using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogRealtime.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserApplication _userApplication;
    private readonly ITokenService _tokenService;

    public AuthController(IUserApplication userApplication, ITokenService tokenService)
    {
        _userApplication = userApplication;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _userApplication.ValidateLogin(dto);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _tokenService.GenerateToken(user);
        var response = new AuthResponseDto(token, new UserDto(user.Id, user.Name, user.Email));

        return Ok(response);
    }
}

