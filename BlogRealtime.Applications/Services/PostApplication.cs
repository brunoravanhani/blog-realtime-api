
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;

namespace BlogRealtime.Application.Services;

public class PostApplication : IPostApplication
{
    private readonly IPostService _postService;

    public PostApplication(IPostService postService)
    {
        _postService = postService;
    }

    public async Task<IEnumerable<PostDto>> GetAll()
    {
        var posts = await _postService.GetAll();
        return posts.Select(p =>
        {
            return new PostDto(p.Id, p.Title, p.Body, p.Image, new AuthorDto(p.Author.Name));
        });
    }

    public async Task<PostDto?> GetById(Guid id)
    {
        var post = await _postService.GetById(id);
        return new PostDto(post.Id, post.Title, post.Body, post.Image, new AuthorDto(post.Author.Name));
    }

    public async Task Add(CreatePostDto createPostDto)
    {
        var post = new Post(createPostDto.Title, createPostDto.Body, createPostDto.Image, createPostDto.UserId);
        await _postService.Add(post);
    }
}
