using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface IMuiTiemRepository
{
    Task<List<LichTiem>> ListLichChoTiemAsync();
    Task<LichTiem?> GetLichAsync(int id);
    Task<List<NguoiDung>> ListNhanVienAsync();
    Task<NguoiDung?> GetNhanVienAsync(int id);
    Task<bool> CoChongChiDinhDaXacNhanAsync(int nguoiTiemId, int vaccineId);
    Task<bool> DaTiemMuiAsync(int nguoiTiemId, int vaccineId, int muiThu);
    Task LuuMuiTiemAsync(MuiTiem mui);          // trừ tồn lô + đổi trạng thái lịch + thêm mũi tiêm
    Task<MuiTiem?> GetMuiAsync(int id);
    Task<List<MuiTiem>> ListGanDayAsync(int soLuong);
}