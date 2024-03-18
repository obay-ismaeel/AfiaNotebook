using AfiaNotebook.DataService.IRepository;

namespace AfiaNotebook.DataService.IConfiguration;
public interface IUnitOfWork
{
    IUsersRepository Users { get; }

    Task CompleteAsync();
}
