using AfiaNotebook.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.DataService.IRepository;
public interface IUsersRepository : IGenericRepository<User>
{
    Task<bool> UpdateUserProfile(User user);
}
