
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogRealtime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostApplication _postApplication;

    public PostController(IPostApplication postApplication)
    {
        _postApplication = postApplication;
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
        await _postApplication.Add(dto);
        return Created();
    }
}
