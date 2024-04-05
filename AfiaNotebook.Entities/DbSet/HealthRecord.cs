using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.Entities.DbSet;
public class HealthRecord : BaseEntity
{
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string BloodType { get; set; }
    public string Race { get; set; }
    public bool UseGlasses { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
