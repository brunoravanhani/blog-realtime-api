using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Domain.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
