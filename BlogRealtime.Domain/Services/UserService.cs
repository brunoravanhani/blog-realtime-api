using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Repository;

namespace BlogRealtime.Domain.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _userRepository.GetByEmail(email) ?? throw new ResourceNotFoundException("User not found");
    }

    public async Task Add(User user)
    {
        await _userRepository.Add(user);
        await _userRepository.SaveChanges();
    }
}
