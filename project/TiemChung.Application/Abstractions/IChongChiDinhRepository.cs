using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface IChongChiDinhRepository
{
    Task<List<ChongChiDinh>> ListAsync(string? trangThai);
    Task<ChongChiDinh?> GetAsync(int id);
    Task<List<ChongChiDinh>> ListTheoNguoiTiemAsync(int nguoiTiemId);
    // Đã có chống chỉ định chưa thu hồi (chưa xác nhận hoặc đã xác nhận) cho cặp người - vaccine này chưa
    Task<bool> TonTaiConHieuLucAsync(int nguoiTiemId, int vaccineId);
    Task ThemAsync(ChongChiDinh c, Func<ChongChiDinh, NhatKyHieuChinh> nhatKy);
    Task SuaAsync(ChongChiDinh c, NhatKyHieuChinh nhatKy);
}
