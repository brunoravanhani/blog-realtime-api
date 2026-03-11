
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;
using FluentValidation;
using FluentValidation.Results;

namespace BlogRealtime.Application.Services;

internal class PostApplication : IPostApplication
{
    private readonly IPostService _postService;
    private readonly IValidator<CreatePostDto> _createPostValidator;
    private readonly IValidator<UpdatePostDto> _updatePostValidator;

    public PostApplication(IPostService postService, IValidator<CreatePostDto> createPostValidator, IValidator<UpdatePostDto> updatePostValidator)
    {
        _postService = postService;
        _createPostValidator = createPostValidator;
        _updatePostValidator = updatePostValidator;
    }

    public async Task<IEnumerable<PostDto>> GetAll()
    {
        var posts = await _postService.GetAll();
        return posts.Select(p =>
        {
            return new PostDto(p.Id, p.Title, p.Body, p.Image, new AuthorDto(p.Author.Name));
        });
    }

    public async Task<PostDto> GetById(Guid id)
    {
        var post = await _postService.GetById(id);
        return new PostDto(post.Id, post.Title, post.Body, post.Image, new AuthorDto(post.Author.Name));
    }

    public async Task Create(CreatePostDto createPostDto, Guid userId)
    {
        ValidationResult validationResult = await _createPostValidator.ValidateAsync(createPostDto);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var post = new Post(createPostDto.Title, createPostDto.Body, createPostDto.Image, userId);
        await _postService.Add(post);
    }

    public async Task Update(Guid id, UpdatePostDto dto)
    {
        ValidationResult validationResult = await _updatePostValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var postToUpdate = new Post(id, dto.Title, dto.Body, dto.Image);
        await _postService.Update(postToUpdate);
    }

    public async Task Delete(Guid id)
    {
        await _postService.Delete(id);
    }
}
