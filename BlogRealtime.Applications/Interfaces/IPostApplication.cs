using BlogRealtime.Domain.Dtos;

namespace BlogRealtime.Application.Interfaces;

public interface IPostApplication
{
    Task<IEnumerable<PostDto>> GetAll();
    Task<PostDto> GetById(Guid id);
    Task Create(CreatePostDto createPostDto, Guid userId);
    Task Update(Guid id, UpdatePostDto dto);
    Task Delete(Guid id);
}
