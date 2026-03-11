
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace BlogRealtime.Application.Services;

internal class PostApplication : IPostApplication
{
    private readonly IPostService _postService;
    private readonly IValidator<CreatePostDto> _createPostValidator;
    private readonly IValidator<UpdatePostDto> _updatePostValidator;
    private readonly ILogger<PostApplication> _logger;

    public PostApplication(IPostService postService, IValidator<CreatePostDto> createPostValidator, IValidator<UpdatePostDto> updatePostValidator,
        ILogger<PostApplication> logger)
    {
        _postService = postService;
        _createPostValidator = createPostValidator;
        _updatePostValidator = updatePostValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<PostDto>> GetAll()
    {
        _logger.LogInformation("Retrieving all posts");
        var posts = await _postService.GetAll();
        _logger.LogInformation("Retrieved {PostCount} posts", posts.Count());
        return posts.Select(p =>
        {
            return new PostDto(p.Id, p.Title, p.Body, p.Image, new AuthorDto(p.Author.Name));
        });
    }

    public async Task<PostDto> GetById(Guid id)
    {
        _logger.LogInformation("Retrieving post with ID: {PostId}", id);
        var post = await _postService.GetById(id);
        _logger.LogInformation("Retrieved post with ID: {PostId}", id);
        return new PostDto(post.Id, post.Title, post.Body, post.Image, new AuthorDto(post.Author.Name));
    }

    public async Task Create(CreatePostDto createPostDto, Guid userId)
    {
        _logger.LogInformation("Attempting to create post. UserId: {UserId}, Title: {Title}", userId, createPostDto.Title);

        ValidationResult validationResult = await _createPostValidator.ValidateAsync(createPostDto);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Post creation validation failed. Errors: {Errors}", 
                string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new ValidationException(validationResult.Errors);
        }

        var post = new Post(createPostDto.Title, createPostDto.Body, createPostDto.Image, userId);
        await _postService.Add(post);

        _logger.LogInformation("Post created successfully. PostId: {PostId}, UserId: {UserId}", post.Id, userId);
    }

    public async Task Update(Guid id, UpdatePostDto dto)
    {
        _logger.LogInformation("Attempting to update post with ID: {PostId}", id);

        ValidationResult validationResult = await _updatePostValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Post update validation failed for PostId: {PostId}. Errors: {Errors}", 
                id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new ValidationException(validationResult.Errors);
        }

        var postToUpdate = new Post(id, dto.Title, dto.Body, dto.Image);
        await _postService.Update(postToUpdate);

        _logger.LogInformation("Post updated successfully. PostId: {PostId}", id);
    }

    public async Task Delete(Guid id)
    {
        _logger.LogInformation("Attempting to delete post with ID: {PostId}", id);
        await _postService.Delete(id);
        _logger.LogInformation("Post deleted successfully. PostId: {PostId}", id);
    }
}
