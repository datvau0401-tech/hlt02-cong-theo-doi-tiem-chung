using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiemChung.Application.Services;
using TiemChung.Domain;

namespace TiemChung.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService service) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        try
        {
            var u = await service.DangNhapAsync(req);
            await GhiCookieAsync(u);
            return Ok(u);
        }
        catch (DomainException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // Người dân tự đăng ký tài khoản, xong là đăng nhập luôn
    [AllowAnonymous]
    [HttpPost("dang-ky")]
    public async Task<IActionResult> DangKy(DangKyRequest req)
    {
        try
        {
            var u = await service.DangKyAsync(req);
            await GhiCookieAsync(u);
            return Created("/api/auth/me", u);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    // Trang web gọi khi mở lên để biết còn đang đăng nhập hay không (chưa đăng nhập -> 401)
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new UserDto(
        User.LayId() ?? 0,
        User.Identity?.Name ?? "",
        User.FindFirstValue(ClaimTypes.Role) ?? ""));

    private Task GhiCookieAsync(UserDto u)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new(ClaimTypes.Name, u.HoTen),
            new(ClaimTypes.Role, u.VaiTro),
        };
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
