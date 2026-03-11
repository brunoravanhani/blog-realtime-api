using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Exceptions;
using BlogRealtime.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace BlogRealtime.Domain.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> GetByEmail(string email)
    {
        _logger.LogInformation("Retrieving user by email: {Email}", email);
        var user = await _userRepository.GetByEmail(email);
        if (user == null)
        {
            _logger.LogWarning("User not found for email: {Email}", email);
            throw new ResourceNotFoundException("User not found");
        }
        _logger.LogInformation("User retrieved successfully. Email: {Email}, UserId: {UserId}", email, user.Id);
        return user;
    }

    public async Task Add(User user)
    {
        _logger.LogInformation("Adding new user. Email: {Email}, Name: {Name}", user.Email, user.Name);
        await _userRepository.Add(user);
        await _userRepository.SaveChanges();
        _logger.LogInformation("User added successfully. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
    }
}
