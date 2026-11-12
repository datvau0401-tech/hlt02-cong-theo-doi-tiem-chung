using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Abstractions;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/vaccine")]
public class VaccineController(IVaccineRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
        => Ok((await repo.ListAsync()).Select(v => new { v.Id, v.Ten }));
}