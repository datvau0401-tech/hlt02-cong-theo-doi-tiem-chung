using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/lich-tiem")]
public class LichTiemController(LichTiemService service) : ControllerBase
{
    // ?trangThai=ChoTiem | DaTiem | DaHuy (bỏ trống = tất cả)
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? trangThai) =>
        Ok(await service.ListAsync(trangThai));

    [HttpPost]
    public async Task<IActionResult> Create(TaoLichRequest req)
    {
        try
        {
            var dto = await service.TaoAsync(req, User.LayId());
            return Created($"/api/lich-tiem/{dto.Id}", dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/ngay-hen")]
    public async Task<IActionResult> DoiNgayHen(int id, DoiNgayHenRequest req)
    {
        try
        {
            return Ok(await service.DoiNgayHenAsync(id, req.NgayHen, req.LyDo, User.LayId()));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/huy")]
    public async Task<IActionResult> Huy(int id, HuyLichRequest req)
    {
        try
        {
            return Ok(await service.HuyAsync(id, req.LyDo, User.LayId()));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
