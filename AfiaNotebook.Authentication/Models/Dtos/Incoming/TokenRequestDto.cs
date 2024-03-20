using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfiaNotebook.Authentication.Models.Dtos.Incoming;
public class TokenRequestDto
{
    [Required]
    public string JwtToken { get; set; }
    
    [Required]
    public string RefreshToken { get; set; }
}
