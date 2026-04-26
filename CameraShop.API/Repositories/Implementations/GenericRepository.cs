using System.Linq.Expressions;
using CameraShop.API.Data;
using CameraShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CameraShop.API.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _set;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _set.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(T entity) => await _set.AddAsync(entity);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public Task<int> SaveAsync() => _context.SaveChangesAsync();
}
