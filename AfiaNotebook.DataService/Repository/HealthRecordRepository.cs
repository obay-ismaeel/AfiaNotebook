using AfiaNotebook.DataService.Data;
using AfiaNotebook.DataService.IRepository;
using AfiaNotebook.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.DataService.Repository;
public class HealthRecordRepository : GenericRepository<HealthRecord>, IHealthRecordRepository
{
    public HealthRecordRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public override async Task<IEnumerable<HealthRecord>> GetAll()
    {
        try
        {
            return await _dbSet.Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetAll method has generated an error", typeof(HealthRecordRepository));
            return new List<HealthRecord>();
        }
    }
}
