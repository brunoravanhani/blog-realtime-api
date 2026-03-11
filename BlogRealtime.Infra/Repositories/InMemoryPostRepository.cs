using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace BlogRealtime.Infra.Repositories;

internal class InMemoryPostRepository : IPostRepository
{
    private readonly BlogDbContext _context;

    public InMemoryPostRepository(BlogDbContext context)
    {
        _context = context;
    }

    public async Task Add(Post post)
    {
        await _context.Posts.AddAsync(post);
    }

    public async Task Delete(Post post)
    {
        _context.Posts.Remove(post);
    }

    public async Task<IEnumerable<Post>> GetAll()
    {
        return await _context.Posts.Include(p => p.Author).ToListAsync();
    }

    public async Task<Post?> GetById(Guid id)
    {
        return await _context.Posts.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

}
