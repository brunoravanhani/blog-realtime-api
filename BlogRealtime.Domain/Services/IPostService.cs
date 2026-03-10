using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Domain.Services;

public interface IPostService
{
    Task<IEnumerable<Post>> GetAll();
    Task<Post?> GetById(Guid id);
    Task Add(Post post);
    Task Delete(Guid id);
    Task SaveChanges();
}
