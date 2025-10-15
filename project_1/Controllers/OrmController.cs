// Controllers/OrmController.cs
using Microsoft.AspNetCore.Mvc;
using project_1.Data;

[ApiController]
[Route("orm")]
public class OrmController(project_1Context db) : ControllerBase
{
    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        var canConnect = await db.Database.CanConnectAsync();
        return canConnect
            ? Ok(new { status = "ok", db = "connected" })
            : StatusCode(503, new { status = "degraded", db = "unreachable" });
    }
}
