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
public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        try
        {
            return await _dbSet.Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetAll method has generated an error", typeof(UsersRepository));
            return new List<User>() ;
        }
    }

    public async Task<User> GetByIdentityId(Guid id)
    {
        try
        {
            return await _dbSet.Where(x => x.Status == 1 )
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdentityId == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetByIdentityId method has generated an error", typeof(UsersRepository));
            return null;
        }
    }

    public async Task<bool> UpdateUserProfile(User user)
    {
        try
        {
            var existingUser = await _dbSet.Where(x => x.Status == 1).FirstOrDefaultAsync(x=> x.Id == user.Id);

            if (existingUser is null) return false;

            existingUser.Sex = user.Sex;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.MobileNumber = user.MobileNumber;
            existingUser.Address = user.Address;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Country = user.Country;
            existingUser.UpdatedAt = DateTime.Now;
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} UpdateUserProfile method has generated an error", typeof(UsersRepository));
            return false;
        }
    }
}
