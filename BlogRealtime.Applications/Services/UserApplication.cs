using BlogRealtime.Application.Interfaces;
using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;

namespace BlogRealtime.Application.Services;

public class UserApplication : IUserApplication
{
    private readonly IUserService _userService;

    public UserApplication(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<User?> ValidateLogin(UserLoginDto dto)
    {
        User user = await _userService.GetByEmail(dto.Email) ?? throw new ArgumentException();

        if (user.ValidatePassword(dto.Password))
        {
            return user;
        }

        return null;
    }

    public async Task Create(CreateUserDto dto)
    {
        var user = new User(dto.Name, dto.Email, dto.Password);
        await _userService.Add(user);
    }
}
