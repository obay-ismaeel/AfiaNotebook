using AfiaNotebook.Authentication.Configuration.Models;
using AfiaNotebook.Authentication.Models.Dtos.Generic;
using AfiaNotebook.Authentication.Models.Dtos.Incoming;
using AfiaNotebook.Authentication.Models.Dtos.Outgoing;
using AfiaNotebook.DataService.IConfiguration;
using AfiaNotebook.Entities.DbSet;
using AutoMapper;
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
    private readonly JwtOptions _jwtOptions;
    private readonly TokenValidationParameters _tokenValidationParameters;
    public AccountsController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IOptionsMonitor<JwtOptions> optionsMonitor, TokenValidationParameters tokenValidationParameters, IMapper mapper) : base(unitOfWork, userManager, mapper)
    {
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
            PhoneNumber = "XXXX",
            Country = "UK",
            MobileNumber = "XXXX", 
            Address = "XX St.", 
            Sex = "Non Binary",
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

    [HttpPost("refreshToken")]
    public async Task<IActionResult> UseRefreshToken(TokenRequestDto tokenRequestDto)
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

        var result = await VerifyToken(tokenRequestDto);

        if(result is null | !result!.Success)
        {
            return BadRequest(new UserRegisterationResponseDto
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Token validation has failed"
                }
            });
        }

        return Ok(result);
    }

    private async Task<AuthResult> VerifyToken(TokenRequestDto tokenRequestDto)
    {
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principle = handler.ValidateToken(tokenRequestDto.JwtToken, _tokenValidationParameters, out var validatedToken);

            // if the string generated is an actual jwt token
            if(validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                // check if the jwt token is created using the same algorithm that we used
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (!result) return null;
            }

            // check expiry date
            var utcExpiryDate = long.Parse(principle.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

            // check if JWT token hasn't expired
            if(expiryDate > DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Jwt Token Has Not Expired!"
                    }
                };
            }

            // check if refresh token exists
            var dbRefreshToken = await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequestDto.RefreshToken);

            if(dbRefreshToken is null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid Refresh Token!"
                    }
                };
            }

            // check refresh token expiry date
            if(dbRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Refresh Token has expired!, please login again."
                    }
                };
            }

            if (dbRefreshToken.IsUsed)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Refresh Token has been used! it can't be reused"
                    }
                };
            }

            if (dbRefreshToken.IsRevoked)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Refresh Token has been revoked! it can't be used."
                    }
                };
            }

            var jti = principle.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            if(dbRefreshToken.JwtId != jti)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Refresh Token reference does not match the jwt token."
                    }
                };
            }

            //start processing and getting new token
            dbRefreshToken.IsUsed = true;
            var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(dbRefreshToken);
            await _unitOfWork.CompleteAsync();

            if (!updateResult)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Error processing the request."
                    }
                };
            }

            var dbUser = await _userManager.FindByIdAsync(dbRefreshToken.UserId);

            if(dbUser is null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Error processing the request."
                    }
                };
            }

            var tokens = await GenerateJwtAndRefreshTokens(dbUser);

            return new AuthResult
            {
                Success = true,
                JwtToken = tokens.JwtToken,
                RefreshToken = tokens.RefreshToken
            };
        }
        catch (Exception ex)
        {
            // todo error handling and logging
            return null;
        }
    }

    private DateTime UnixTimeStampToDateTime(long unixDate)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();

        return dateTime;
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
                new Claim(ClaimTypes.NameIdentifier, user.Id),
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
