
using Microsoft.AspNetCore.Mvc;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;

namespace BlogRealtime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> Get()
    {
        return Ok(await _postService.GetAll());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Post>> GetById(Guid id)
    {
        var post = await _postService.GetById(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    public record CreatePostDto(string Title, string Body, string Image, Guid UserId);

    [HttpPost]
    public async Task<ActionResult<Post>> Create([FromBody] CreatePostDto dto)
    {
        var post = new Post(dto.Title, dto.Body, dto.Image ?? string.Empty, dto.UserId);
        await _postService.Add(post);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }
}
