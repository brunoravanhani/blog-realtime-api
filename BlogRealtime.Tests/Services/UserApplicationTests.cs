using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Cryptography;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValResult = FluentValidation.Results.ValidationResult;
using FluentValException = FluentValidation.ValidationException;

namespace BlogRealtime.Tests.Services;

public class UserApplicationTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ICryptographyHelper> _cryptographyHelperMock;
    private readonly Mock<IValidator<UserLoginDto>> _userLoginValidatorMock;
    private readonly Mock<IValidator<CreateUserDto>> _createUserValidatorMock;
    private readonly IUserApplication _userApplication;

    public UserApplicationTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _cryptographyHelperMock = new Mock<ICryptographyHelper>();
        _userLoginValidatorMock = new Mock<IValidator<UserLoginDto>>();
        _createUserValidatorMock = new Mock<IValidator<CreateUserDto>>();

        // Create instance using reflection with NullLogger for internal class
        var userAppType = typeof(IUserApplication).Assembly.GetType("BlogRealtime.Application.Services.UserApplication")!;
        var nullLoggerType = typeof(NullLogger<>).MakeGenericType(userAppType);
        var logger = Activator.CreateInstance(nullLoggerType)!;

        _userApplication = (IUserApplication)Activator.CreateInstance(
            userAppType,
            _userServiceMock.Object,
            _cryptographyHelperMock.Object,
            _userLoginValidatorMock.Object,
            _createUserValidatorMock.Object,
            logger
        )!;
    }

    #region ValidateLogin Tests

    [Fact]
    public async Task ValidateLogin_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var faker = new Faker();
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var hashedPassword = faker.Random.Hash();

        var loginDto = new UserLoginDto(email, password);
        var user = new User(faker.Name.FirstName(), email, hashedPassword);

        _userLoginValidatorMock
            .Setup(v => v.ValidateAsync(loginDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _userServiceMock
            .Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _cryptographyHelperMock
            .Setup(c => c.VerifyPassword(password, hashedPassword))
            .Returns(true);

        // Act
        var result = await _userApplication.ValidateLogin(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Id, result.Id);

        _userServiceMock.Verify(s => s.GetByEmail(email), Times.Once);
        _cryptographyHelperMock.Verify(c => c.VerifyPassword(password, hashedPassword), Times.Once);
    }

    [Fact]
    public async Task ValidateLogin_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var faker = new Faker();
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var hashedPassword = faker.Random.Hash();

        var loginDto = new UserLoginDto(email, password);
        var user = new User(faker.Name.FirstName(), email, hashedPassword);

        _userLoginValidatorMock
            .Setup(v => v.ValidateAsync(loginDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _userServiceMock
            .Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _cryptographyHelperMock
            .Setup(c => c.VerifyPassword(password, hashedPassword))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _userApplication.ValidateLogin(loginDto)
        );
        
        Assert.Equal("Username or password invalid", exception.Message);
    }

    [Fact]
    public async Task ValidateLogin_WithNonExistentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var faker = new Faker();
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();

        var loginDto = new UserLoginDto(email, password);

        _userLoginValidatorMock
            .Setup(v => v.ValidateAsync(loginDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _userServiceMock
            .Setup(s => s.GetByEmail(email))
            .ThrowsAsync(new ResourceNotFoundException("User not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _userApplication.ValidateLogin(loginDto)
        );
        
        Assert.Equal("Username or password invalid", exception.Message);
    }

    [Fact]
    public async Task ValidateLogin_WithValidationErrors_ThrowsValidationException()
    {
        // Arrange
        var faker = new Faker();
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();

        var loginDto = new UserLoginDto(email, password);
        var validationFailures = new List<ValidationFailure>
        {
            new("Email", "Email is required"),
            new("Password", "Password is required")
        };

        _userLoginValidatorMock
            .Setup(v => v.ValidateAsync(loginDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult(validationFailures));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValException>(
            () => _userApplication.ValidateLogin(loginDto)
        );

        Assert.NotEmpty(exception.Errors);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_CreatesUserSuccessfully()
    {
        // Arrange
        var faker = new Faker();
        var name = faker.Name.FirstName();
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var hashedPassword = faker.Random.Hash();

        var createUserDto = new CreateUserDto(name, email, password);

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(createUserDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _cryptographyHelperMock
            .Setup(c => c.HashPassword(password))
            .Returns(hashedPassword);

        _userServiceMock
            .Setup(s => s.Add(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userApplication.Create(createUserDto);

        // Assert
        _cryptographyHelperMock.Verify(c => c.HashPassword(password), Times.Once);
        _userServiceMock.Verify(s => s.Add(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithValidationErrors_ThrowsValidationException()
    {
        // Arrange
        var faker = new Faker();
        var createUserDto = new CreateUserDto("", faker.Internet.Email(), faker.Internet.Password());
        var validationFailures = new List<ValidationFailure>
        {
            new("Name", "Name is required")
        };

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(createUserDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult(validationFailures));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValException>(
            () => _userApplication.Create(createUserDto)
        );

        Assert.NotEmpty(exception.Errors);
        _userServiceMock.Verify(s => s.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithMultipleValidationErrors_ThrowsValidationException()
    {
        // Arrange
        var createUserDto = new CreateUserDto("", "", "");
        var validationFailures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Email", "Email is required"),
            new("Password", "Password is required")
        };

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(createUserDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult(validationFailures));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValException>(
            () => _userApplication.Create(createUserDto)
        );

        Assert.Equal(3, exception.Errors.Count());
    }

    [Fact]
    public async Task Create_WithServiceException_PropagatesToCaller()
    {
        // Arrange
        var faker = new Faker();
        var createUserDto = new CreateUserDto(faker.Name.FirstName(), faker.Internet.Email(), faker.Internet.Password());
        var hashedPassword = faker.Random.Hash();

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(createUserDto, CancellationToken.None))
            .ReturnsAsync(new FluentValResult());

        _cryptographyHelperMock
            .Setup(c => c.HashPassword(It.IsAny<string>()))
            .Returns(hashedPassword);

        _userServiceMock
            .Setup(s => s.Add(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userApplication.Create(createUserDto));
    }

    #endregion
}
