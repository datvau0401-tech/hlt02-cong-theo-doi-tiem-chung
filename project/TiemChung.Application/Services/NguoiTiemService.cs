using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record NguoiTiemRequest(string? HoTen, DateOnly NgaySinh, string? GioiTinh,
                               string? SoDienThoai, string? Email, string? Cccd = null);
public record NguoiTiemDto(int Id, string HoTen, DateOnly NgaySinh, string? GioiTinh,
                           string? SoDienThoai, string? Email, string? Cccd = null);

public class NguoiTiemService(INguoiTiemRepository repo)
{
    public async Task<List<NguoiTiemDto>> ListAsync()
        => (await repo.ListAsync()).Select(ToDto).ToList();

    public async Task<NguoiTiemDto> TaoAsync(NguoiTiemRequest r, int? nguoiSuaId)
    {
        var n = NguoiTiem.TaoMoi(r.HoTen, r.NgaySinh, r.GioiTinh, r.SoDienThoai, r.Email, r.Cccd);   // quy tắc kiểm ở Domain
        await repo.ThemAsync(n, x => NhatKyHieuChinh.Tao(
            "NguoiTiem", x.Id.ToString(), HanhDongNhatKy.Them, null, NhatKyJson.Tao(Snap(x)), nguoiSuaId, null));
        return ToDto(n);
    }

    public async Task<NguoiTiemDto> SuaAsync(int id, NguoiTiemRequest r, int? nguoiSuaId)
    {
        var n = await repo.GetAsync(id) ?? throw new DomainException("Người tiêm không tồn tại.");

        var cu = NhatKyJson.Tao(Snap(n));
        n.CapNhat(r.HoTen, r.NgaySinh, r.GioiTinh, r.SoDienThoai, r.Email, r.Cccd);
        var moi = NhatKyJson.Tao(Snap(n));

        if (cu == moi) return ToDto(n);   // không có gì thay đổi thì không ghi nhật ký

        await repo.SuaAsync(n, NhatKyHieuChinh.Tao(
            "NguoiTiem", id.ToString(), HanhDongNhatKy.Sua, cu, moi, nguoiSuaId, null));
        return ToDto(n);
    }

    private static object Snap(NguoiTiem n) =>
        new { n.HoTen, n.NgaySinh, n.GioiTinh, n.SoDienThoai, n.Email, n.Cccd };

    public static NguoiTiemDto ToDto(NguoiTiem n) =>
        new(n.Id, n.HoTen, n.NgaySinh, n.GioiTinh, n.SoDienThoai, n.Email, n.Cccd);
}
