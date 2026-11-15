using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

// Chỉ người dân, và chỉ thấy/sửa dữ liệu của chính mình (id lấy từ cookie, không nhận từ trang web)
[ApiController]
[Authorize(Roles = "NguoiDan")]
[Route("api/cua-toi")]
public class CuaToiController(CaNhanService service) : ControllerBase
{
    private int Toi => User.LayId() ?? 0;

    [HttpGet("thong-tin")]
    public Task<IActionResult> ThongTin() => Chay(async () => Ok(await service.ThongTinAsync(Toi)));

    [HttpGet("lich")]
    public Task<IActionResult> Lich() => Chay(async () => Ok(await service.LichAsync(Toi)));

    [HttpPost("lich")]
    public Task<IActionResult> DatLich(DatLichCuaToiRequest req) =>
        Chay(async () => Ok(await service.DatLichAsync(Toi, req)));

    [HttpPost("lich/{id:int}/huy")]
    public Task<IActionResult> HuyLich(int id, HuyLichRequest req) =>
        Chay(async () => Ok(await service.HuyLichAsync(Toi, id, req.LyDo)));

    [HttpGet("mui-tiem")]
    public Task<IActionResult> MuiTiem() => Chay(async () => Ok(await service.MuiDaTiemAsync(Toi)));

    [HttpGet("chong-chi-dinh")]
    public Task<IActionResult> ChongChiDinh() => Chay(async () => Ok(await service.ChongChiDinhAsync(Toi)));

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
