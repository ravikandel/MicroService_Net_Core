using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces(MediaTypeNames.Application.Json)]

    public class BaseController : ControllerBase
    {

    }
}
