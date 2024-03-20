using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfiaNotebook.Entities.DbSet;
public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } // user id when logged in
    public string Token { get; set; }
    public string JwtId { get; set; } // id generated when a jwt token is created
    public bool IsUsed { get; set; } // to make sure r.token is used only once
    public bool IsRevoked { get; set; } // make sure they are valid
    public DateTime ExpiryDate { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }
}
