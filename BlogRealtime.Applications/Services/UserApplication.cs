using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Cryptography;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System;

namespace BlogRealtime.Application.Services;

internal class UserApplication : IUserApplication
{
    private readonly IUserService _userService;
    private readonly ICryptographyHelper _cryptographyHelper;
    private readonly IValidator<UserLoginDto> _userLoginValidator;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly ILogger<UserApplication> _logger;

    public UserApplication(IUserService userService, ICryptographyHelper cryptographyHelper, 
        IValidator<UserLoginDto> userLoginValidator,
        IValidator<CreateUserDto> createUserValidator,
        ILogger<UserApplication> logger)
    {
        _userService = userService;
        _cryptographyHelper = cryptographyHelper;
        _userLoginValidator = userLoginValidator;
        _createUserValidator = createUserValidator;
        _logger = logger;
    }

    public async Task<User?> ValidateLogin(UserLoginDto dto)
    {
        _logger.LogInformation("Attempting login for email: {Email}", dto.Email);

        ValidationResult validationResult = await _userLoginValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Login validation failed for email: {Email}. Errors: {Errors}", 
                dto.Email, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new ValidationException(validationResult.Errors);
        }

        try
        {
            User user = await _userService.GetByEmail(dto.Email);
            if (!_cryptographyHelper.VerifyPassword(dto.Password, user.Password))
            {
                _logger.LogWarning("Invalid password for email: {Email}", dto.Email);
                throw new UnauthorizedAccessException("Username or password invalid");
            }
            _logger.LogInformation("User login successful for email: {Email}", dto.Email);
            return user;
        }
        catch (ResourceNotFoundException)
        {
            _logger.LogWarning("User not found for email: {Email}", dto.Email);
            throw new UnauthorizedAccessException("Username or password invalid");
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException)
        {
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", dto.Email);
            throw;
        }
    }

    public async Task Create(CreateUserDto dto)
    {
        _logger.LogInformation("Attempting to create user with email: {Email}", dto.Email);

        ValidationResult validationResult = await _createUserValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("User creation validation failed for email: {Email}. Errors: {Errors}", 
                dto.Email, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new ValidationException(validationResult.Errors);
        }

        var hashedPassword = _cryptographyHelper.HashPassword(dto.Password);

        var user = new User(dto.Name, dto.Email, hashedPassword);
        await _userService.Add(user);

        _logger.LogInformation("User created successfully with email: {Email}", dto.Email);
    }
}
