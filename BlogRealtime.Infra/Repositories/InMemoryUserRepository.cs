using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace BlogRealtime.Infra.Repositories;

internal class InMemoryUserRepository : IUserRepository
{
    private readonly BlogDbContext _context;

    public InMemoryUserRepository(BlogDbContext context)
    {
        _context = context;
    }


    public async Task<User?> GetByEmail(string email) => await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetById(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }   
}
