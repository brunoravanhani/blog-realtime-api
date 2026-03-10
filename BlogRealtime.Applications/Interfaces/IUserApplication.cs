using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Application.Interfaces;

public interface IUserApplication
{
    Task<User?> ValidateLogin(UserLoginDto dto);
    Task Create(CreateUserDto dto);
}
