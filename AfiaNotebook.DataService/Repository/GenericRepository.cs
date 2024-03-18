using AfiaNotebook.DataService.Data;
using AfiaNotebook.DataService.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AfiaNotebook.DataService.Repository;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ILogger _logger;
    protected readonly AppDbContext _context;
    protected DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<bool> Add(T entity)
    {
        await _dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> Delete(Guid id, string userId)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<IEnumerable<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> GetById(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<bool> Upsert(T entity)
    {
        throw new NotImplementedException();
    }
}
