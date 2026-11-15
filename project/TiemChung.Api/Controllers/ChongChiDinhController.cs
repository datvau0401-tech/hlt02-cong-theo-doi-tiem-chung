using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/chong-chi-dinh")]
public class ChongChiDinhController(ChongChiDinhService service) : ControllerBase
{
    // ?trangThai=ChuaXacNhan | DaXacNhan | DaThuHoi (bỏ trống = tất cả)
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? trangThai) =>
        Ok(await service.ListAsync(trangThai));

    [HttpPost]
    public Task<IActionResult> Create(TaoCcdRequest req) =>
        Chay(async () =>
        {
            var dto = await service.TaoAsync(req, User.LayId());
            return Created($"/api/chong-chi-dinh/{dto.Id}", dto);
        });

    [HttpPut("{id:int}")]
    public Task<IActionResult> SuaLyDo(int id, SuaCcdRequest req) =>
        Chay(async () => Ok(await service.SuaLyDoAsync(id, req.LyDo, User.LayId())));

    [HttpPost("{id:int}/xac-nhan")]
    public Task<IActionResult> XacNhan(int id) =>
        Chay(async () => Ok(await service.XacNhanAsync(id, User.LayId())));

    [HttpPost("{id:int}/thu-hoi")]
    public Task<IActionResult> ThuHoi(int id, ThuHoiCcdRequest req) =>
        Chay(async () => Ok(await service.ThuHoiAsync(id, req.LyDo, User.LayId())));

    private async Task<IActionResult> Chay(Func<Task<IActionResult>> viec)
    {
        try
        {
            return await viec();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
