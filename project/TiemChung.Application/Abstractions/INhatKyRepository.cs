using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface INhatKyRepository
{
    Task<List<NhatKyHieuChinh>> ListAsync(string? tenBang, string? tuKhoa, int soLuong);
}
