using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/lo-vaccine")]
public class LoVaccineController(LoVaccineService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await service.ListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateLoVaccineRequest req)
    {
        try
        {
            var dto = await service.CreateAsync(req);
            return Created($"/api/lo-vaccine/{dto.Id}", dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}