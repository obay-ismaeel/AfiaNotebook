using AfiaNotebook.DataService.IConfiguration;
using AfiaNotebook.Entities.DbSet;
using AfiaNotebook.Entities.Dtos.Errors;
using AfiaNotebook.Entities.Dtos.Generic;
using AfiaNotebook.Entities.Dtos.Incoming;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AfiaNotebook.Api.Controllers.v1;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _unitOfWork.Users.GetAll();
        var successResult = new PageResult<User> 
        { 
            Content = users.ToList(), 
            ResultCount = users.Count() 
        };
        return Ok(successResult);
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

        return CreatedAtAction(nameof(GetUser), new { id = _user.Id }, user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _unitOfWork.Users.GetById(id);

        if (user is null)
        {
            var result = new Result<User> { Error = new Error(404, "Not found", "No such user") };
            return NotFound(result);
        }

        var successResult = new Result<User> { Content = user };
        return Ok(successResult);
    }
}
