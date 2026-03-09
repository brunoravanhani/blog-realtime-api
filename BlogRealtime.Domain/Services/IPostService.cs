using BlogRealtime.Domain.Dtos;
using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Domain.Services;

public interface IPostService
{
    Task<IEnumerable<PostDto>> GetAll();
    Task<PostDto?> GetById(Guid id);
    Task Add(Post post);
    Task Delete(Guid id);
}
