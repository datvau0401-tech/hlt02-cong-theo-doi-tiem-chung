using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(DashboardService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await service.LayAsync());
}
