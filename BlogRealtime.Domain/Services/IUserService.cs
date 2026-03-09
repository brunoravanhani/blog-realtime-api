using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Domain.Services;

public interface IUserService
{
    Task<User?> ValidateLogin(UserLoginDto dto);
    Task Add(User user);
}
