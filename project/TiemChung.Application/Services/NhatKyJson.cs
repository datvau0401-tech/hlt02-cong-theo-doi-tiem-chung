using System.Text.Encodings.Web;
using System.Text.Json;

namespace TiemChung.Application.Services;

// Chụp trạng thái một bản ghi thành JSON để lưu vào nhật ký hiệu chỉnh
public static class NhatKyJson
{
    private static readonly JsonSerializerOptions Opt = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,   // giữ nguyên chữ tiếng Việt, không đổi thành \uXXXX
    };

    public static string Tao(object anh) => JsonSerializer.Serialize(anh, Opt);
}
