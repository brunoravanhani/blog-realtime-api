using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValResult = FluentValidation.Results.ValidationResult;
using FluentValException = FluentValidation.ValidationException;

namespace BlogRealtime.Tests.Services;

public class PostApplicationTests
{
    private readonly Mock<IPostService> _postServiceMock;
    private readonly Mock<IValidator<CreatePostDto>> _createPostValidatorMock;
    private readonly Mock<IValidator<UpdatePostDto>> _updatePostValidatorMock;
    private readonly IPostApplication _postApplication;

    public PostApplicationTests()
    {
        _postServiceMock = new Mock<IPostService>();
        _createPostValidatorMock = new Mock<IValidator<CreatePostDto>>();
        _updatePostValidatorMock = new Mock<IValidator<UpdatePostDto>>();

        // Create instance using reflection with NullLogger for internal class
        var postAppType = typeof(IPostApplication).Assembly.GetType("BlogRealtime.Application.Services.PostApplication")!;
        var nullLoggerType = typeof(NullLogger<>).MakeGenericType(postAppType);
        var logger = Activator.CreateInstance(nullLoggerType)!;

        _postApplication = (IPostApplication)Activator.CreateInstance(
            postAppType,
            _postServiceMock.Object,
            _createPostValidatorMock.Object,
            _updatePostValidatorMock.Object,
            logger
        )!;
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WithPosts_ReturnsAllPostDtos()
    {
        // Arrange
        var faker = new Faker();
        var posts = new List<Post>();
        for (int i = 0; i < 3; i++)
        {
            var author = new User(faker.Name.FirstName(), faker.Internet.Email(), faker.Random.Hash());
            var post = new Post(
                faker.Lorem.Sentence(),
                faker.Lorem.Paragraphs(),
                faker.Image.PicsumUrl(),
                Guid.NewGuid()
            );

            // Set Author using reflection since it's internal
            var authorProperty = typeof(Post).GetProperty("Author", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            authorProperty?.SetValue(post, author);

            posts.Add(post);
        }

        _postServiceMock
            .Setup(s => s.GetAll())
            .ReturnsAsync(posts);

        // Act
        var result = await _postApplication.GetAll();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
        _postServiceMock.Verify(s => s.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAll_WithNoPosts_ReturnsEmptyCollection()
    {
        // Arrange
        _postServiceMock
            .Setup(s => s.GetAll())
            .ReturnsAsync(new List<Post>());

        // Act
        var result = await _postApplication.GetAll();

        // Assert
        Assert.Empty(result);
        _postServiceMock.Verify(s => s.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAll_MapsPostsToPostDtosCorrectly()
    {
        // Arrange
        var faker = new Faker();
        var author = new User(faker.Name.FirstName(), faker.Internet.Email(), faker.Random.Hash());
        var post = new Post(
            faker.Random.Guid(),
            faker.Lorem.Sentence(),
            faker.Lorem.Paragraphs(),
            faker.Image.PicsumUrl()
        );

        // Set Author using reflection since it's internal
        var authorProperty = typeof(Post).GetProperty("Author", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        authorProperty?.SetValue(post, author);

        var posts = new List<Post> { post };

        _postServiceMock
            .Setup(s => s.GetAll())
            .ReturnsAsync(posts);

        // Act
        var result = await _postApplication.GetAll();

        // Assert
        Assert.Single(result);
        Assert.Equal(author.Name, result.First().Author.Name);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsPostDto()
    {
        // Arrange
        var faker = new Faker();
        var postId = Guid.NewGuid();
        var author = new User(faker.Name.FirstName(), faker.Internet.Email(), faker.Random.Hash());
        var post = new Post(postId, "Test Title", "Test Body", "test-image.jpg");

        // Set Author using reflection since it's internal
        var authorProperty = typeof(Post).GetProperty("Author", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        authorProperty?.SetValue(post, author);

        _postServiceMock
            .Setup(s => s.GetById(postId))
            .ReturnsAsync(post);

        // Act
        var result = await _postApplication.GetById(postId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(postId, result.Id);
        Assert.Equal("Test Title", result.title);
        Assert.Equal(author.Name, result.Author.Name);
        _postServiceMock.Verify(s => s.GetById(postId), Times.Once);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var postId = Guid.NewGuid();

        _postServiceMock
            .Setup(s => s.GetById(postId))
            .ThrowsAsync(new ResourceNotFoundException("Post not found"));

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _postApplication.GetById(postId)
        );
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_CreatesPostSuccessfully()
    {
        // Arrange
        var faker = new Faker();
        var userId = Guid.NewGuid();
        var createPostDto = new CreatePostDto(
            faker.Lorem.Sentence(),
            faker.Lorem.Paragraphs(),
            faker.Image.PicsumUrl()
        );

        _createPostValidatorMock
            .Setup(v => v.ValidateAsync(createPostDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _postServiceMock
            .Setup(s => s.Add(It.IsAny<Post>()))
            .Returns(Task.CompletedTask);

        // Act
        await _postApplication.Create(createPostDto, userId);

        // Assert
        _postServiceMock.Verify(s => s.Add(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithValidationErrors_ThrowsValidationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createPostDto = new CreatePostDto("", "", "");
        var validationFailures = new List<ValidationFailure>
        {
            new("Title", "Title is required")
        };

        _createPostValidatorMock
            .Setup(v => v.ValidateAsync(createPostDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult(validationFailures));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValException>(
            () => _postApplication.Create(createPostDto, userId)
        );

        Assert.NotEmpty(exception.Errors);
        _postServiceMock.Verify(s => s.Add(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithMultipleValidationErrors_ThrowsValidationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createPostDto = new CreatePostDto("", "", "");
        var validationFailures = new List<ValidationFailure>
        {
            new("Title", "Title is required"),
            new("Body", "Body is required"),
            new("Image", "Image is required")
        };

        _createPostValidatorMock
            .Setup(v => v.ValidateAsync(createPostDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult(validationFailures));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValException>(
            () => _postApplication.Create(createPostDto, userId)
        );

        Assert.Equal(3, exception.Errors.Count());
    }

    [Fact]
    public async Task Create_WithServiceException_PropagatesToCaller()
    {
        // Arrange
        var faker = new Faker();
        var userId = Guid.NewGuid();
        var createPostDto = new CreatePostDto(
            faker.Lorem.Sentence(),
            faker.Lorem.Paragraphs(),
            faker.Image.PicsumUrl()
        );

        _createPostValidatorMock
            .Setup(v => v.ValidateAsync(createPostDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _postServiceMock
            .Setup(s => s.Add(It.IsAny<Post>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _postApplication.Create(createPostDto, userId)
        );
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidData_UpdatesPostSuccessfully()
    {
        // Arrange
        var faker = new Faker();
        var postId = Guid.NewGuid();
        var updatePostDto = new UpdatePostDto(
            faker.Lorem.Sentence(),
            faker.Lorem.Paragraphs(),
            faker.Image.PicsumUrl()
        );

        _updatePostValidatorMock
            .Setup(v => v.ValidateAsync(updatePostDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _postServiceMock
            .Setup(s => s.Update(It.IsAny<Post>()))
            .Returns(Task.CompletedTask);

        // Act
        await _postApplication.Update(postId, updatePostDto);

        // Assert
        _postServiceMock.Verify(s => s.Update(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task Update_WithValidationErrors_ThrowsValidationException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var updatePostDto = new UpdatePostDto("", "", "");
        var validationFailures = new List<ValidationFailure>
        {
            new("Title", "Title is required")
        };

        _updatePostValidatorMock
            .Setup(v => v.ValidateAsync(updatePostDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult(validationFailures));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValException>(
            () => _postApplication.Update(postId, updatePostDto)
        );

        Assert.NotEmpty(exception.Errors);
        _postServiceMock.Verify(s => s.Update(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task Update_WithServiceException_ThrowsResourceNotFoundException()
    {
        // Arrange
        var faker = new Faker();
        var postId = Guid.NewGuid();
        var updatePostDto = new UpdatePostDto(
            faker.Lorem.Sentence(),
            faker.Lorem.Paragraphs(),
            faker.Image.PicsumUrl()
        );

        _updatePostValidatorMock
            .Setup(v => v.ValidateAsync(updatePostDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _postServiceMock
            .Setup(s => s.Update(It.IsAny<Post>()))
            .ThrowsAsync(new ResourceNotFoundException("Post not found"));

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _postApplication.Update(postId, updatePostDto)
        );
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_DeletesPostSuccessfully()
    {
        // Arrange
        var postId = Guid.NewGuid();

        _postServiceMock
            .Setup(s => s.Delete(postId))
            .Returns(Task.CompletedTask);

        // Act
        await _postApplication.Delete(postId);

        // Assert
        _postServiceMock.Verify(s => s.Delete(postId), Times.Once);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var postId = Guid.NewGuid();

        _postServiceMock
            .Setup(s => s.Delete(postId))
            .ThrowsAsync(new ResourceNotFoundException("Post not found"));

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _postApplication.Delete(postId)
        );
    }

    #endregion

    #region Helper Methods

    private List<Post> GenerateFakePosts(int count)
    {
        var faker = new Faker();
        var posts = new List<Post>();

        for (int i = 0; i < count; i++)
        {
            var post = new Post(
                faker.Lorem.Sentence(),
                faker.Lorem.Paragraphs(),
                faker.Image.PicsumUrl(),
                Guid.NewGuid()
            );
            posts.Add(post);
        }

        return posts;
    }

    #endregion
}
