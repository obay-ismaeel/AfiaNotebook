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

    public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
    {
        try
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.Token.ToLower() == refreshToken.ToLower());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetByRefreshToken method has generated an error", typeof(RefreshTokenRepository));
            return null;
        }
    }

    public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken dbRefreshToken)
    {
        try
        {
            var token = await _dbSet.SingleOrDefaultAsync(x => x.Id == dbRefreshToken.Id);

            if (token is null) 
                return false;

            token.IsUsed = true;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} MarkRefreshTokenAsUsed method has generated an error", typeof(RefreshTokenRepository));
            return null;
        }
    }
}
