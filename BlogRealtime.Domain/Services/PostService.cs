using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace BlogRealtime.Domain.Services;

internal class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostService> _logger;

    public PostService(IPostRepository postRepository, ILogger<PostService> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Post>> GetAll()
    {
        _logger.LogInformation("Retrieving all posts from repository");
        var posts = await _postRepository.GetAll();
        _logger.LogInformation("Retrieved {PostCount} posts from repository", posts.Count());
        return posts;
    }

    public async Task<Post> GetById(Guid id)
    {
        _logger.LogInformation("Retrieving post by ID: {PostId}", id);
        var post = await _postRepository.GetById(id);
        if (post == null)
        {
            _logger.LogWarning("Post not found for ID: {PostId}", id);
            throw new ResourceNotFoundException("Post not found");
        }
        _logger.LogInformation("Post retrieved successfully. PostId: {PostId}", id);
        return post;
    }

    public async Task Add(Post post)
    {
        _logger.LogInformation("Adding new post. Title: {Title}, UserId: {UserId}", post.Title, post.UserId);
        await _postRepository.Add(post);
        await _postRepository.SaveChanges();
        _logger.LogInformation("Post added successfully. PostId: {PostId}, Title: {Title}", post.Id, post.Title);
    }

    public async Task Update(Post post)
    {
        _logger.LogInformation("Updating post. PostId: {PostId}, Title: {Title}", post.Id, post.Title);
        var postOld = await _postRepository.GetById(post.Id);
        if (postOld == null)
        {
            _logger.LogWarning("Post not found for update. PostId: {PostId}", post.Id);
            throw new ResourceNotFoundException("Post not found");
        }

        postOld.ChangeTitle(post.Title);
        postOld.ChangeBody(post.Body);
        postOld.ChangeImage(post.Image);

        await _postRepository.SaveChanges();
        _logger.LogInformation("Post updated successfully. PostId: {PostId}", post.Id);
    }

    public async Task Delete(Guid id)
    {
        _logger.LogInformation("Deleting post. PostId: {PostId}", id);
        var post = await _postRepository.GetById(id);
        if (post == null)
        {
            _logger.LogWarning("Post not found for deletion. PostId: {PostId}", id);
            throw new ResourceNotFoundException("Post not found");
        }
        await _postRepository.Delete(post);
        await _postRepository.SaveChanges();
        _logger.LogInformation("Post deleted successfully. PostId: {PostId}", id);
    }
}
