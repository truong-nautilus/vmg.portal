using Microsoft.AspNetCore.Mvc;
using System;

namespace ServerCore.PortalAPI.Presentation.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "UP", timestamp = DateTime.UtcNow });
        }
    }
}
