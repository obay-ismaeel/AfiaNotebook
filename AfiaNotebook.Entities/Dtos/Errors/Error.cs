using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.Entities.Dtos.Errors;
public class Error
{
    public int Code { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }

    public Error(int code, string type, string message)
    {
        Code = code;
        Type = type;
        Message = message;
    }
}
