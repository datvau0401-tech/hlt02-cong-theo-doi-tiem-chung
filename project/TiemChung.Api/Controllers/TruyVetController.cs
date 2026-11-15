using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/truy-vet")]
public class TruyVetController(TruyVetService service) : ControllerBase
{
    [HttpGet("lo/{loId:int}")]
    public async Task<IActionResult> TheoLo(int loId)
    {
        try { return Ok(await service.TheoLoAsync(loId)); }
        catch (DomainException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("nguoi-tiem/{id:int}")]
    public async Task<IActionResult> TheoNguoiTiem(int id)
    {
        try { return Ok(await service.TheoNguoiTiemAsync(id)); }
        catch (DomainException ex) { return NotFound(new { message = ex.Message }); }
    }
}
