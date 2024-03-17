using AfiaNotebook.Entities.DbSet;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.DataService.Data;
public class AppDbContext : IdentityDbContext
{
    public virtual DbSet<User> Users {  get; set; }
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
}
