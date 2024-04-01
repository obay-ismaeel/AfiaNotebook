using AfiaNotebook.Entities.Dtos.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.Entities.Dtos.Generic;
public class Result<T>
{
    public T Content { get; set; }
    public Error  Error { get; set; }
    public bool IsSuccessful => Error == null;
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
}
