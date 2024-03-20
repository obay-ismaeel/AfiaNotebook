using AfiaNotebook.DataService.Data;
using AfiaNotebook.DataService.IRepository;
using AfiaNotebook.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AfiaNotebook.DataService.Repository;
public class RefreshTokenRepository :  GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public override async Task<IEnumerable<RefreshToken>> GetAll()
    {
        try
        {
            return await _dbSet.Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetAll method has generated an error", typeof(RefreshTokenRepository));
            return new List<RefreshToken>();
        }
    }
}
