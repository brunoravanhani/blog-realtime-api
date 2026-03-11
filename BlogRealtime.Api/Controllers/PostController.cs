
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BlogRealtime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostApplication _postApplication;
    private readonly WebSockets.WebSocketsManager _webSocketManager;
    private readonly ILogger<PostController> _logger;

    public PostController(IPostApplication postApplication, WebSockets.WebSocketsManager webSocketManager, ILogger<PostController> logger)
    {
        _postApplication = postApplication;
        _webSocketManager = webSocketManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> Get()
    {
        _logger.LogInformation("PostController - Retrieving all posts");
        try
        {
            var posts = await _postApplication.GetAll();
            _logger.LogInformation("PostController - Retrieved posts successfully");
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostController - Error retrieving all posts");
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostDto>> GetById(Guid id)
    {
        _logger.LogInformation("PostController - Retrieving post with ID: {PostId}", id);
        try
        {
            var post = await _postApplication.GetById(id);
            if (post == null)
            {
                _logger.LogWarning("PostController - Post not found. PostId: {PostId}", id);
                return NotFound();
            }
            _logger.LogInformation("PostController - Post retrieved successfully. PostId: {PostId}", id);
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostController - Error retrieving post with ID: {PostId}", id);
            throw;
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Post>> Create([FromBody] CreatePostDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User id not found in claims");
        _logger.LogInformation("PostController - Creating post. UserId: {UserId}, Title: {Title}", userId, dto.Title);

        try
        {
            await _postApplication.Create(dto, Guid.Parse(userId));

            await _webSocketManager.BroadcastAsync($"New Post Created: {dto.Title}");

            _logger.LogInformation("PostController - Post created successfully. UserId: {UserId}, Title: {Title}", userId, dto.Title);
            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostController - Error creating post. UserId: {UserId}, Title: {Title}", userId, dto.Title);
            throw;
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Post>> Update(Guid id, [FromBody] UpdatePostDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("PostController - Updating post. PostId: {PostId}, UserId: {UserId}, Title: {Title}", id, userId, dto.Title);

        try
        {
            await _postApplication.Update(id, dto);
            _logger.LogInformation("PostController - Post updated successfully. PostId: {PostId}", id);
            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostController - Error updating post. PostId: {PostId}", id);
            throw;
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("PostController - Deleting post. PostId: {PostId}, UserId: {UserId}", id, userId);

        try
        {
            await _postApplication.Delete(id);
            _logger.LogInformation("PostController - Post deleted successfully. PostId: {PostId}", id);
            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostController - Error deleting post. PostId: {PostId}", id);
            throw;
        }
    }
}
