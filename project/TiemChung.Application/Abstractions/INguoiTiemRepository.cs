using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface INguoiTiemRepository
{
    Task<List<NguoiTiem>> ListAsync();
    Task<NguoiTiem?> GetAsync(int id);
    // Lưu người tiêm mới + ghi nhật ký trong 1 transaction (nhật ký cần Id vừa được tạo)
    Task ThemAsync(NguoiTiem n, Func<NguoiTiem, NhatKyHieuChinh> nhatKy);
    // n là đối tượng đang được theo dõi và đã sửa xong; lưu cùng nhật ký trong 1 lần lưu
    Task SuaAsync(NguoiTiem n, NhatKyHieuChinh nhatKy);
}
