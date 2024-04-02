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
public class ProfileController : BaseController
{
    public ProfileController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var loggedUser = await _userManager.GetUserAsync(HttpContext.User);

        if(loggedUser == null)
        {
            var result = new Result<User> { Error = new Error(400, "User not found", "Bad request") };
            return BadRequest(result);
        }

        var identityId = new Guid(loggedUser.Id);

        var user = await _unitOfWork.Users.GetByIdentityId(identityId);

        if (user == null)
        {
            var result = new Result<User> { Error = new Error(400, "User not found", "Bad request") };
            return BadRequest(result);
        }

        var successResult = new Result<User> { Content = user };
        return Ok(successResult);
    }

    [HttpPut]   
    public async Task<IActionResult> UpdateProfile(ProfileDto profileDto)
    {
        if (!ModelState.IsValid)
        {
            var result = new Result<User> { Error = new Error(400, "Invalid Payload", "Bad request") };
            return BadRequest(result);
        }

        var loggedUser = await _userManager.GetUserAsync(HttpContext.User);

        if (loggedUser == null)
        {
            var result = new Result<User> { Error = new Error(400, "User not found", "Bad request") };
            return BadRequest(result);
        }

        var identityId = new Guid(loggedUser.Id);

        var user = await _unitOfWork.Users.GetByIdentityId(identityId);

        if (user == null)
        {
            var result = new Result<User> { Error = new Error(400, "User not found", "Bad request") };
            return BadRequest(result);
        }

        user.Address = profileDto.Address;
        user.MobileNumber = profileDto.MobileNumber;
        user.Country = profileDto.Country;
        user.Sex = profileDto.Sex;

        var isUpdated = await _unitOfWork.Users.UpdateUserProfile(user);

        if (!isUpdated)
        {
            var result = new Result<User> { Error = new Error(500, "Something went wrong", "Server Error") };
            return StatusCode(500,result);
        }

        await _unitOfWork.CompleteAsync();

        var successResult = new Result<User> { Content = user };
        return Ok(successResult);
    }
}
