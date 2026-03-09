using System;
using System.Collections.Generic;
using BlogRealtime.Domain.Entity;

namespace BlogRealtime.Domain.Repository;

public interface IUserRepository : IRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(Guid id);
    Task<User>? GetByEmail(string email);
    Task Add(User user);
}
