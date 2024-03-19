using AfiaNotebook.DataService.IConfiguration;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AfiaNotebook.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion(1)] 
public class BaseController : ControllerBase
{
    protected readonly IUnitOfWork _unitOfWork;

    public BaseController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
}
