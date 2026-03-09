using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Repository;

namespace BlogRealtime.Domain.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<IEnumerable<PostDto>> GetAll()
    {
        var posts = await _postRepository.GetAll();
        return posts.Select(p =>
        {
            return new PostDto(p.Id, p.Title, p.Body, p.Image, new AuthorDto(p.Author.Name));
        });
    }

    public async Task<PostDto?> GetById(Guid id)
    {
        var post =  await _postRepository.GetById(id);
        return new PostDto(post.Id, post.Title, post.Body, post.Image, new AuthorDto(post.Author.Name));
    }

    public async Task Add(Post post)
    {
        await _postRepository.Add(post);
        await _postRepository.SaveChanges();
    }

    public async Task Delete(Guid id)
    {
        await _postRepository.Delete(id);
        await _postRepository.SaveChanges();
    }
}
