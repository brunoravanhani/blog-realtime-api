using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Repository;

namespace BlogRealtime.Domain.Services;

internal class PostService : IPostService
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

    public async Task<Post> GetById(Guid id)
    {
        return await _postRepository.GetById(id) ?? throw new ResourceNotFoundException("Post not found");
    }

    public async Task Add(Post post)
    {
        await _postRepository.Add(post);
        await _postRepository.SaveChanges();
    }

    public async Task Update(Post post)
    {
        var postOld = await _postRepository.GetById(post.Id) ?? throw new ResourceNotFoundException("Post not found");

        postOld.ChangeTitle(post.Title);
        postOld.ChangeBody(post.Body);
        postOld.ChangeImage(post.Image);

        await _postRepository.SaveChanges();
    }

    public async Task Delete(Guid id)
    {
        var post = await _postRepository.GetById(id) ?? throw new ResourceNotFoundException("Post not found");
        await _postRepository.Delete(post);
        await _postRepository.SaveChanges();
    }
}
