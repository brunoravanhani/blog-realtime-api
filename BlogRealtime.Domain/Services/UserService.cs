using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Repository;

namespace BlogRealtime.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> ValidateLogin(UserLoginDto dto)
    {
        User user = await _userRepository.GetByEmail(dto.Email) ?? throw new ArgumentException();

        if (user.ValidatePassword(dto.Password))
        {
            return user;
        }

        return null;
    }

    public async Task Add(User user)
    {
        await _userRepository.Add(user);
        await _userRepository.SaveChanges();
    }
}
