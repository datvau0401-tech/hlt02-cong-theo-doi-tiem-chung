using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record GhiBienDongRequest(int LoVaccineId, string Loai, int SoLuong, int NguoiThucHienId, string? LyDo);
public record BienDongDto(long Id, string SoLo, string Vaccine, string Loai, string TenLoai,
                          int SoLuong, DateTime ThoiDiem, string NguoiThucHien, string LyDo);

public class BienDongLoService(IBienDongLoRepository repo, ILoVaccineRepository lots, IMuiTiemRepository nhanVien)
{
    public async Task<List<BienDongDto>> GanDayAsync()
        => (await repo.ListGanDayAsync(20)).Select(ToDto).ToList();

    public async Task<BienDongDto> GhiNhanAsync(GhiBienDongRequest r)
    {
        var lo = await lots.GetAsync(r.LoVaccineId)
                 ?? throw new DomainException("Lô vaccine không tồn tại.");
        var nv = await nhanVien.GetNhanVienAsync(r.NguoiThucHienId)
                 ?? throw new DomainException("Người thực hiện phải là nhân viên y tế.");

        var bd = BienDongLo.GhiNhan(lo, r.Loai, r.SoLuong, nv.Id, r.LyDo);  // quy tắc kiểm ở Domain
        await repo.LuuAsync(bd);                                             // lưu trong 1 transaction
        return ToDto((await repo.GetAsync(bd.Id))!);
    }

    private static string TenLoai(string loai) => loai switch
    {
        LoaiBienDong.NhapThem => "Nhập thêm",
        LoaiBienDong.Hong => "Hỏng",
        LoaiBienDong.HaoHut => "Hao hụt",
        LoaiBienDong.Huy => "Hủy",
        _ => loai,
    };

    private static BienDongDto ToDto(BienDongLo b) => new(
        b.Id, b.LoVaccine?.SoLo ?? "", b.LoVaccine?.Vaccine?.Ten ?? "", b.Loai, TenLoai(b.Loai),
        b.SoLuong, b.ThoiDiem, b.NguoiThucHien?.HoTen ?? "", b.LyDo);
}