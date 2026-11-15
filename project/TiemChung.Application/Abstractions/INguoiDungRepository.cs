using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface INguoiDungRepository
{
    Task<NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap);
    Task<NguoiDung?> GetAsync(int id);                                  // đối tượng được theo dõi để sửa
    Task<List<NguoiDung>> ListAsync();
    Task<bool> TenDangNhapDaCoAsync(string tenDangNhap, int? boQuaId);

    // Người dân tự đăng ký: tạo hồ sơ người tiêm + tài khoản + nhật ký trong 1 transaction
    Task<NguoiDung> DangKyNguoiDanAsync(NguoiTiem nguoiTiem, Func<NguoiTiem, NguoiDung> taoTaiKhoan,
                                        Func<NguoiTiem, NguoiDung, NhatKyHieuChinh[]> nhatKy);
    Task ThemAsync(NguoiDung u, Func<NguoiDung, NhatKyHieuChinh> nhatKy);
    Task SuaAsync(NguoiDung u, NhatKyHieuChinh nhatKy);
}
