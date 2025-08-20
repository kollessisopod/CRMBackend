using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRMBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    //health check get request
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Service is running" });
    }

}
