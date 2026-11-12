using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api")]
public class MuiTiemController(MuiTiemService service) : ControllerBase
{
    [HttpGet("lich-tiem/cho-tiem")]
    public async Task<IActionResult> LichChoTiem() => Ok(await service.LichChoTiemAsync());

    [HttpGet("nhan-vien")]
    public async Task<IActionResult> NhanVien() => Ok(await service.NhanVienAsync());

    [HttpGet("mui-tiem")]
    public async Task<IActionResult> GanDay() => Ok(await service.GanDayAsync());

    [HttpPost("mui-tiem")]
    public async Task<IActionResult> GhiNhan(GhiNhanMuiTiemRequest req)
    {
        try
        {
            var dto = await service.GhiNhanAsync(req);
            return Created($"/api/mui-tiem/{dto.Id}", dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}