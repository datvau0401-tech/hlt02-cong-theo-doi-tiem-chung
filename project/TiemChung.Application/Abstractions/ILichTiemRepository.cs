using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface ILichTiemRepository
{
    Task<List<LichTiem>> ListAsync(string? trangThai);
    Task<LichTiem?> GetAsync(int id);
    Task<List<LichTiem>> ListTheoNguoiTiemAsync(int nguoiTiemId);
    Task<bool> TonTaiLichConHieuLucAsync(int nguoiTiemId, int vaccineId, int muiThu);
    // Ngày mốc để tính khoảng cách giữa 2 mũi: ngày đã tiêm mũi trước, hoặc ngày hẹn nếu chưa tiêm.
    // Trả về null nếu mũi trước chưa có lịch nào còn hiệu lực.
    Task<DateOnly?> NgayMocMuiTruocAsync(int nguoiTiemId, int vaccineId, int muiTruoc);
    Task<bool> CoChongChiDinhDaXacNhanAsync(int nguoiTiemId, int vaccineId);
    Task ThemAsync(LichTiem l, Func<LichTiem, NhatKyHieuChinh> nhatKy);
    Task SuaAsync(LichTiem l, NhatKyHieuChinh nhatKy);
}
