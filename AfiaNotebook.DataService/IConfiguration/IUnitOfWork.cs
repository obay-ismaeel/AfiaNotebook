using AfiaNotebook.DataService.IRepository;

namespace AfiaNotebook.DataService.IConfiguration;
public interface IUnitOfWork
{
    IUsersRepository Users { get; }

    IRefreshTokenRepository RefreshTokens { get; }

    Task CompleteAsync();
}
