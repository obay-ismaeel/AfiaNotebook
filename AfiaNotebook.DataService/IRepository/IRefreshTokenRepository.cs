using AfiaNotebook.Entities.DbSet;

namespace AfiaNotebook.DataService.IRepository;
public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken> GetByRefreshToken(string refreshToken);
    Task<bool> MarkRefreshTokenAsUsed(RefreshToken dbRefreshToken);
}
