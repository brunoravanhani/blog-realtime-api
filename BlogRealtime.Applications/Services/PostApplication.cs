
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;

namespace BlogRealtime.Application.Services;

internal class PostApplication : IPostApplication
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
        var post = await _postService.GetById(id) ?? throw new InvalidOperationException("Post not found");
        return new PostDto(post.Id, post.Title, post.Body, post.Image, new AuthorDto(post.Author.Name));
    }

    public async Task Create(CreatePostDto createPostDto, Guid userId)
    {
        var post = new Post(createPostDto.Title, createPostDto.Body, createPostDto.Image, userId);
        await _postService.Add(post);
    }

    public async Task Update(Guid id, UpdatePostDto dto)
    {
        var post = await _postService.GetById(id) ?? throw new InvalidOperationException("Post not found");

        post.ChangeTitle(dto.Title);
        post.ChangeBody(dto.Body);
        post.ChangeImage(dto.Image);

        await _postService.SaveChanges();
    }

    public async Task Delete(Guid id)
    {
        await _postService.Delete(id);

        await _postService.SaveChanges();
    }
}
