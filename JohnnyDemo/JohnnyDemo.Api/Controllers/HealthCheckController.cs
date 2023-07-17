using Microsoft.AspNetCore.Mvc;

namespace JohnnyDemo.Api.Controllers
{
    /// <summary>
    /// Health check controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Retrive status of service
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Healthy";
        }
    }
}
