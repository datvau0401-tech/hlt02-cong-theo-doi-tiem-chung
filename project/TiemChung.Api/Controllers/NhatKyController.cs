using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/nhat-ky")]
public class NhatKyController(NhatKyService service) : ControllerBase
{
    // ?tenBang=NguoiTiem&tuKhoa=...&soLuong=100
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? tenBang, [FromQuery] string? tuKhoa,
                                          [FromQuery] int soLuong = 100) =>
        Ok(await service.ListAsync(tenBang, tuKhoa, soLuong));
}
