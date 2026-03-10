
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace BlogRealtime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostApplication _postApplication;
    private readonly WebSockets.WebSocketsManager _webSocketManager;

    public PostController(IPostApplication postApplication, WebSockets.WebSocketsManager webSocketManager)
    {
        _postApplication = postApplication;
        _webSocketManager = webSocketManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> Get()
    {
        return Ok(await _postApplication.GetAll());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostDto>> GetById(Guid id)
    {
        var post = await _postApplication.GetById(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Post>> Create([FromBody] CreatePostDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User id not found in claims");

        await _postApplication.Create(dto, Guid.Parse(userId));

        await _webSocketManager.BroadcastAsync($"New Post Created: {dto.Title}");

        return Created();
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Post>> Update(Guid id, [FromBody] UpdatePostDto dto)
    {
        await _postApplication.Update(id, dto);
        return Created();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _postApplication.Delete(id);
        return Created();
    }
}
