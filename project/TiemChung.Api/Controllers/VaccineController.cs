using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/vaccine")]
public class VaccineController(VaccineService service) : ControllerBase
{
    [Authorize]   // người dân cũng cần xem danh mục để đặt lịch
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await service.ListAsync());

    // Danh mục vaccine chỉ quản trị viên được thêm/sửa
    [Authorize(Roles = "QuanTri")]
    [HttpPost]
    public async Task<IActionResult> Create(VaccineRequest req)
    {
        try
        {
            var dto = await service.TaoAsync(req, User.LayId());
            return Created($"/api/vaccine/{dto.Id}", dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "QuanTri")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VaccineRequest req)
    {
        try
        {
            return Ok(await service.SuaAsync(id, req, User.LayId()));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
