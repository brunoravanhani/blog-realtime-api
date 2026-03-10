using Microsoft.AspNetCore.Mvc;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Application.Interfaces;

namespace BlogRealtime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserApplication _userApplication;

    public UserController(IUserApplication userApplication)
    {
        _userApplication = userApplication;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        await _userApplication.Create(dto);
        return Created();
    }
}
