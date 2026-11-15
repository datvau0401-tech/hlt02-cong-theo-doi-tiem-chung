using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record LoginRequest(string? TenDangNhap, string? MatKhau, string? Cccd = null);
public record DangKyRequest(string? HoTen, DateOnly NgaySinh, string? GioiTinh, string? SoDienThoai, string? Email,
                            string? TenDangNhap, string? MatKhau, string? Cccd = null);
public record UserDto(int Id, string HoTen, string VaiTro);

// Quy tắc mật khẩu dùng chung cho đăng ký và cấp tài khoản
public static class QuyTacMatKhau
{
    public static string Kiem(string? matKhau)
    {
        matKhau ??= "";
        if (matKhau.Length is < 8 or > 100 || !matKhau.Any(char.IsLetter) || !matKhau.Any(char.IsDigit))
            throw new DomainException("Mật khẩu phải từ 8 ký tự trở lên, gồm cả chữ và số.");
        return matKhau;
    }
}

public class AuthService(INguoiDungRepository repo)
{
    public async Task<UserDto> DangNhapAsync(LoginRequest r)
    {
        var ten = (r.TenDangNhap ?? "").Trim();
        var u = ten.Length == 0 ? null : await repo.GetByTenDangNhapAsync(ten);

        // Luôn băm/so sánh một lần, kể cả khi không có tài khoản, để thời gian phản hồi như nhau
        var dung = MatKhauHasher.Verify(r.MatKhau ?? "", u?.MatKhauHash);
        if (u is null || !dung)
            throw new DomainException("Sai tên đăng nhập hoặc mật khẩu.");

        return new UserDto(u.Id, u.HoTen, u.VaiTro);
    }

    // Người dân tự đăng ký: luôn là vai trò NguoiDan, không cho tự chọn vai trò
    public async Task<UserDto> DangKyAsync(DangKyRequest r)
    {
        var ten = NguoiDung.KiemTenDangNhap(r.TenDangNhap);
        var matKhau = QuyTacMatKhau.Kiem(r.MatKhau);
        var nguoiTiem = NguoiTiem.TaoMoi(r.HoTen, r.NgaySinh, r.GioiTinh, r.SoDienThoai, r.Email, r.Cccd);   // kiểm hồ sơ ở Domain

        if (await repo.TenDangNhapDaCoAsync(ten, null))
            throw new DomainException("Tên đăng nhập đã có người dùng.");

        var hash = MatKhauHasher.Hash(matKhau);
        var u = await repo.DangKyNguoiDanAsync(
            nguoiTiem,
            nt => NguoiDung.TaoNguoiDan(nguoiTiem.HoTen, ten, hash, nt.Id),
            (nt, nd) =>
            [
                NhatKyHieuChinh.Tao("NguoiTiem", nt.Id.ToString(), HanhDongNhatKy.Them, null,
                    NhatKyJson.Tao(new { nt.HoTen, nt.NgaySinh, nt.GioiTinh, nt.SoDienThoai, nt.Email, nt.Cccd }), nd.Id, "Tự đăng ký"),
                NhatKyHieuChinh.Tao("NguoiDung", nd.Id.ToString(), HanhDongNhatKy.Them, null,
                    NhatKyJson.Tao(new { nd.HoTen, nd.VaiTro, nd.TenDangNhap }), nd.Id, "Tự đăng ký"),
            ]);
        return new UserDto(u.Id, u.HoTen, u.VaiTro);
    }
}
