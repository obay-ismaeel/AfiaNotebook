using AfiaNotebook.DataService.Data;
using AfiaNotebook.DataService.IConfiguration;
using AfiaNotebook.Entities.DbSet;
using AfiaNotebook.Entities.Dtos.Incoming;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AfiaNotebook.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUnitOfWork _unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _unitOfWork.Users.GetAll();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(UserDto user)
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
        await _unitOfWork.Users.Add(_user);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetUser), new { id = _user.Id } , user); 
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _unitOfWork.Users.GetById(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }
}
