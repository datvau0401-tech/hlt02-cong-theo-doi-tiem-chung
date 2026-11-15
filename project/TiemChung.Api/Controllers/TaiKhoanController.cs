using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Authorize(Roles = "QuanTri")]
[Route("api/tai-khoan")]
public class TaiKhoanController(TaiKhoanService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await service.ListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(TaoTaiKhoanRequest req)
    {
        try
        {
            var dto = await service.TaoAsync(req, User.LayId());
            return Created($"/api/tai-khoan/{dto.Id}", dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/dang-nhap")]
    public async Task<IActionResult> DatDangNhap(int id, DatDangNhapRequest req)
    {
        try
        {
            return Ok(await service.DatDangNhapAsync(id, req, User.LayId()));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
