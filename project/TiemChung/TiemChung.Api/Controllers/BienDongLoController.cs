using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/bien-dong-lo")]
public class BienDongLoController(BienDongLoService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GanDay() => Ok(await service.GanDayAsync());

    [HttpPost]
    public async Task<IActionResult> GhiNhan(GhiBienDongRequest req)
    {
        try
        {
            var dto = await service.GhiNhanAsync(req);
            return Created($"/api/bien-dong-lo/{dto.Id}", dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}