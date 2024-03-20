namespace AfiaNotebook.Authentication.Configuration.Models;

public class JwtOptions
{
    public string Secret { get; set; }
    public TimeSpan ExpiryTimeFrame { get; set; }
}
