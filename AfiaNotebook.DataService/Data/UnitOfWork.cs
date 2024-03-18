using AfiaNotebook.DataService.IConfiguration;
using AfiaNotebook.DataService.IRepository;
using AfiaNotebook.DataService.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.DataService.Data;
internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;
    public IUsersRepository Users {get; private set;}

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("db_logs");
        Users = new UsersRepository(_context, _logger);
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
