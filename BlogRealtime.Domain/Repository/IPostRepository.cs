using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Domain.Repository;

public interface IPostRepository : IRepository
{
    Task<IEnumerable<Post>> GetAll();
    Task<Post?> GetById(Guid id);
    Task Add(Post post);
    Task Delete(Guid id);
}
