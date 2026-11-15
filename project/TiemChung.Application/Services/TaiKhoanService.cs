using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record TaoTaiKhoanRequest(string? HoTen, string? VaiTro, string? TenDangNhap, string? MatKhau);
public record DatDangNhapRequest(string? TenDangNhap, string? MatKhau);
public record TaiKhoanDto(int Id, string HoTen, string VaiTro, string TenVaiTro, string? TenDangNhap, bool CoMatKhau);

// Quản trị viên cấp tài khoản cho bác sĩ / quản trị viên và đặt lại mật khẩu
public class TaiKhoanService(INguoiDungRepository repo)
{
    public async Task<List<TaiKhoanDto>> ListAsync() => (await repo.ListAsync()).Select(ToDto).ToList();

    public async Task<TaiKhoanDto> TaoAsync(TaoTaiKhoanRequest r, int? nguoiSuaId)
    {
        var ten = NguoiDung.KiemTenDangNhap(r.TenDangNhap);
        var matKhau = QuyTacMatKhau.Kiem(r.MatKhau);
        if (await repo.TenDangNhapDaCoAsync(ten, null))
            throw new DomainException("Tên đăng nhập đã có người dùng.");

        var u = NguoiDung.TaoNhanSu(r.HoTen, r.VaiTro, ten, MatKhauHasher.Hash(matKhau));
        await repo.ThemAsync(u, x => NhatKyHieuChinh.Tao(
            "NguoiDung", x.Id.ToString(), HanhDongNhatKy.Them, null, NhatKyJson.Tao(Snap(x)), nguoiSuaId, null));
        return ToDto(u);
    }

    // Đặt tên đăng nhập (nếu cần) và mật khẩu mới cho một tài khoản có sẵn
    public async Task<TaiKhoanDto> DatDangNhapAsync(int id, DatDangNhapRequest r, int? nguoiSuaId)
    {
        var u = await repo.GetAsync(id) ?? throw new DomainException("Tài khoản không tồn tại.");
        var matKhau = QuyTacMatKhau.Kiem(r.MatKhau);
        var ten = string.IsNullOrWhiteSpace(r.TenDangNhap) ? null : NguoiDung.KiemTenDangNhap(r.TenDangNhap);
        if (ten is not null && await repo.TenDangNhapDaCoAsync(ten, id))
            throw new DomainException("Tên đăng nhập đã có người dùng.");

        var cu = NhatKyJson.Tao(Snap(u));
        u.DatDangNhap(ten, MatKhauHasher.Hash(matKhau));
        await repo.SuaAsync(u, NhatKyHieuChinh.Tao(
            "NguoiDung", id.ToString(), HanhDongNhatKy.Sua, cu, NhatKyJson.Tao(Snap(u)), nguoiSuaId, "Đặt tài khoản/mật khẩu đăng nhập"));
        return ToDto(u);
    }

    private static object Snap(NguoiDung u) => new { u.HoTen, u.VaiTro, u.TenDangNhap };

    private static string TenVaiTro(string v) => v switch
    {
        VaiTroNguoiDung.QuanTri => "Quản trị viên",
        VaiTroNguoiDung.NhanVienYTe => "Bác sĩ / nhân viên y tế",
        VaiTroNguoiDung.NguoiDan => "Người dân",
        _ => v,
    };

    private static TaiKhoanDto ToDto(NguoiDung u) =>
        new(u.Id, u.HoTen, u.VaiTro, TenVaiTro(u.VaiTro), u.TenDangNhap, !string.IsNullOrEmpty(u.MatKhauHash));
}
