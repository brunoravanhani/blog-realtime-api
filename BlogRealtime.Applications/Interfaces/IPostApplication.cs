using BlogRealtime.Domain.Dtos;

namespace BlogRealtime.Application.Interfaces;

public interface IPostApplication
{
    Task<IEnumerable<PostDto>> GetAll();
    Task<PostDto?> GetById(Guid id);
    Task Add(CreatePostDto createPostDto);
}
