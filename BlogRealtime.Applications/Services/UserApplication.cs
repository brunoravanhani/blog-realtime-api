using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Cryptography;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Services;
using FluentValidation;
using FluentValidation.Results;
using System;

namespace BlogRealtime.Application.Services;

internal class UserApplication : IUserApplication
{
    private readonly IUserService _userService;
    private readonly ICryptographyHelper _cryptographyHelper;
    private readonly IValidator<UserLoginDto> _userLoginValidator;
    private readonly IValidator<CreateUserDto> _createUserValidator;

    public UserApplication(IUserService userService, ICryptographyHelper cryptographyHelper, 
        IValidator<UserLoginDto> userLoginValidator,
        IValidator<CreateUserDto> createUserValidator)
    {
        _userService = userService;
        _cryptographyHelper = cryptographyHelper;
        _userLoginValidator = userLoginValidator;
        _createUserValidator = createUserValidator;
    }

    public async Task<User?> ValidateLogin(UserLoginDto dto)
    {
        ValidationResult validationResult = await _userLoginValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        try
        {
            User user = await _userService.GetByEmail(dto.Email);
            if (!_cryptographyHelper.VerifyPassword(dto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Username or password invalid");
            }
            return user;
        }
        catch (ResourceNotFoundException)
        {
            throw new UnauthorizedAccessException("Username or password invalid");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task Create(CreateUserDto dto)
    {
        ValidationResult validationResult = await _createUserValidator.ValidateAsync(dto);

        if (!validationResult.IsValid) 
            throw new ValidationException(validationResult.Errors);

        var hashedPassword = _cryptographyHelper.HashPassword(dto.Password);

        var user = new User(dto.Name, dto.Email, hashedPassword);
        await _userService.Add(user);
    }
}
