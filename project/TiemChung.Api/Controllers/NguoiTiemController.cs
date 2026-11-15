using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/nguoi-tiem")]
public class NguoiTiemController(NguoiTiemService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await service.ListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(NguoiTiemRequest req)
    {
        try
        {
            var dto = await service.TaoAsync(req, User.LayId());
            return Created($"/api/nguoi-tiem/{dto.Id}", dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, NguoiTiemRequest req)
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
