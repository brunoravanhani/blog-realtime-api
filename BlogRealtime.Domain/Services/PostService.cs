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

    public async Task<IEnumerable<Post>> GetAll()
    {
        return await _postRepository.GetAll();
    }

    public async Task<Post?> GetById(Guid id)
    {
        return await _postRepository.GetById(id);
    }

    public async Task Add(Post post)
    {
        await _postRepository.Add(post);
        await _postRepository.SaveChanges();
    }

    public async Task Delete(Guid id)
    {
        await _postRepository.Delete(id);
    }

    public async Task SaveChanges()
    {
        await _postRepository.SaveChanges();
    }
}
