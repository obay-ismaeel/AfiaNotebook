using AfiaNotebook.DataService.IConfiguration;
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
            return BadRequest("User not found");
        }

        var identityId = new Guid(loggedUser.Id);

        var user = await _unitOfWork.Users.GetByIdentityId(identityId);

        if (user == null)
        {
            return BadRequest("User not found");
        }

        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(ProfileDto profileDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid Payload");
        }

        var loggedUser = await _userManager.GetUserAsync(HttpContext.User);

        if (loggedUser == null)
        {
            return BadRequest("User not found");
        }

        var identityId = new Guid(loggedUser.Id);

        var user = await _unitOfWork.Users.GetByIdentityId(identityId);

        if (user == null)
        {
            return BadRequest("User not found");
        }

        user.Address = profileDto.Address;
        user.MobileNumber = profileDto.MobileNumber;
        user.Country = profileDto.Country;
        user.Sex = profileDto.Sex;

        var isUpdated = await _unitOfWork.Users.UpdateUserProfile(user);

        if (!isUpdated)
        {
            return BadRequest("Something went wrong");
        }

        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
