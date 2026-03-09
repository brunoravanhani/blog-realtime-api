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

    public async Task<User?> GetByEmail(string email)
    {
        return await _userRepository.GetByEmail(email);
    }

    public async Task Add(User user)
    {
        await _userRepository.Add(user);
        await _userRepository.SaveChanges();
    }
}
