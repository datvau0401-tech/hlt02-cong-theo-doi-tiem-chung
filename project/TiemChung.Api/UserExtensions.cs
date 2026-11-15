using System.Security.Claims;

namespace TiemChung.Api;

public static class UserExtensions
{
    // Id của người đang đăng nhập (lấy từ cookie), dùng để ghi vào nhật ký hiệu chỉnh
    public static int? LayId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
