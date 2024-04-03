using AfiaNotebook.DataService.IConfiguration;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AfiaNotebook.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion(1)] 
public class BaseController : ControllerBase
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly UserManager<IdentityUser> _userManager;
    protected readonly IMapper _mapper;
    public BaseController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }
}
