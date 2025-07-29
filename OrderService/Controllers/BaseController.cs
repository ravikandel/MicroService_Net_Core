using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces(MediaTypeNames.Application.Json)]

    public class BaseController : ControllerBase
    {

    }
}
