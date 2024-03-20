using AfiaNotebook.Authentication.Configuration.Models;
using AfiaNotebook.Authentication.Models.Dtos.Generic;
using AfiaNotebook.Authentication.Models.Dtos.Incoming;
using AfiaNotebook.Authentication.Models.Dtos.Outgoing;
using AfiaNotebook.DataService.IConfiguration;
using AfiaNotebook.Entities.DbSet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AfiaNotebook.Api.Controllers.v1;

public class AccountsController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtOptions _jwtOptions;
    private readonly TokenValidationParameters _tokenValidationParameters;
    public AccountsController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IOptionsMonitor<JwtOptions> optionsMonitor, TokenValidationParameters tokenValidationParameters) : base(unitOfWork)
    {
        _userManager = userManager;
        _jwtOptions = optionsMonitor.CurrentValue;
        _tokenValidationParameters = tokenValidationParameters;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterationRequestDto registerationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserRegisterationResponseDto
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Invalid Payload"
                }
            });
        }

        var user = await _userManager.FindByEmailAsync(registerationDto.Email);

        if (user is not null)
        {
            return BadRequest(new UserRegisterationResponseDto
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Email already exists"
                }
            });
        }

        var newUser = new IdentityUser()
        {
            Email = registerationDto.Email,
            UserName = registerationDto.Email,
            EmailConfirmed = true
        };

        var isCreated = await _userManager.CreateAsync(newUser, registerationDto.Password);

        if (!isCreated.Succeeded)
        {
            return BadRequest(new UserRegisterationResponseDto
            {
                Success = isCreated.Succeeded,
                Errors = isCreated.Errors.Select(x => x.Description).ToList()
            }) ;
        }

        var _user = new User()
        {
            IdentityId = new Guid(newUser.Id),
            FirstName = registerationDto.FirstName,
            LastName = registerationDto.LastName,
            Email = registerationDto.Email,
            PhoneNumber = "",
            Country = "",
            DateOfBirth = DateTime.Now,
            Status = 1
        };
        await _unitOfWork.Users.Add(_user);
        await _unitOfWork.CompleteAsync();

        var tokens = await GenerateJwtAndRefreshTokens(newUser);

        return Ok(new UserRegisterationResponseDto
        {
            Success = true,
            JwtToken = tokens.JwtToken,
            RefreshToken = tokens.RefreshToken
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequestDto loginDto) 
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new UserLoginResponseDto
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Invalid Payload"
                }
            });
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
        {
            return BadRequest(new UserLoginResponseDto
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Invalid authentication request"
                }
            });
        }

        var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isValid)
        {
            return BadRequest(new UserLoginResponseDto
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Invalid password!"
                }
            });
        }

        var tokens = await GenerateJwtAndRefreshTokens(user);

        return Ok(new UserLoginResponseDto
        {
            Success = true,
            JwtToken = tokens.JwtToken,
            RefreshToken = tokens.RefreshToken
        });
    }

    private async Task<TokenData> GenerateJwtAndRefreshTokens(IdentityUser user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email), // unique id
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // used by the refresh token
            }),
            Expires = DateTime.UtcNow.Add(_jwtOptions.ExpiryTimeFrame),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = handler.CreateToken(descriptor);

        var jwtToken = handler.WriteToken(securityToken);

        //generate the refresh token
        var refreshToken = new RefreshToken
        {
            Token = $"{GenerateRandomString(25)}_{Guid.NewGuid()}",
            UserId = user.Id,
            IsRevoked = false,
            IsUsed = false,
            JwtId = securityToken.Id,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
        };

        await _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.CompleteAsync();

        return new TokenData { JwtToken = jwtToken, RefreshToken = refreshToken.Token };
    }

    private string GenerateRandomString(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(36)]).ToArray());
    }
}
