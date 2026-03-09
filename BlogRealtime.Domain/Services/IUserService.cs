using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Domain.Services;

public interface IUserService
{
    Task<User?> GetByEmail(string email);
    Task Add(User user);
}
