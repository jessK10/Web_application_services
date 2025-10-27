using Microsoft.AspNetCore.Mvc;
using project_1.Data;

namespace project_1.Controllers
{
    [ApiController]
    [Route("healthz/db")]
    public class HealthDbController : ControllerBase
    {
        private readonly project_1Context _db;
        public HealthDbController(project_1Context db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> CheckDb()
        {
            try
            {
                var can = await _db.Database.CanConnectAsync();
                return can
                    ? Ok(new { status = "ok", db = "connected" })
                    : StatusCode(503, new { status = "degraded", db = "unreachable" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", db = ex.Message });
            }
        }
    }
}
