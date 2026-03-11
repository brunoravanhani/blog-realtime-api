using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Cryptography;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Services;

namespace BlogRealtime.Application.Services;

internal class UserApplication : IUserApplication
{
    private readonly IUserService _userService;
    private readonly ICryptographyHelper _cryptographyHelper;

    public UserApplication(IUserService userService, ICryptographyHelper cryptographyHelper)
    {
        _userService = userService;
        _cryptographyHelper = cryptographyHelper;
    }

    public async Task<User?> ValidateLogin(UserLoginDto dto)
    {
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
        var hashedPassword = _cryptographyHelper.HashPassword(dto.Password);

        var user = new User(dto.Name, dto.Email, hashedPassword);
        await _userService.Add(user);
    }
}
