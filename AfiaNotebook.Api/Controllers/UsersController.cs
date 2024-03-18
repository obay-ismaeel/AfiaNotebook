using AfiaNotebook.DataService.Data;
using AfiaNotebook.Entities.DbSet;
using AfiaNotebook.Entities.Dtos.Incoming;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AfiaNotebook.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(AppDbContext _context) : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _context.Users.Where(u => u.Status == 1).ToList();
        return Ok(users);
    }

    [HttpPost]
    public IActionResult AddUser(UserDto user)
    {
        var _user = new User()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Country = user.Country,
            DateOfBirth = Convert.ToDateTime(user.DateOfBirth),
            Status = 1
        };
        _context.Users.Add(_user);
        _context.SaveChanges();

        return Ok("added successfully"); //return 201
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(Guid id)
    {
        var user = _context.Users.Find(id);
        
        if (user is null)
            return NotFound();
        
        return Ok(user);
    }
}
