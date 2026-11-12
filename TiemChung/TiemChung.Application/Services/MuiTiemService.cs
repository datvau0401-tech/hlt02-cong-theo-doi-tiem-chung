using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record GhiNhanMuiTiemRequest(int LichTiemId, int LoVaccineId, int NhanVienId);
public record LichChoTiemDto(int Id, string NguoiTiem, int VaccineId, string Vaccine, int MuiThu, DateOnly NgayHen);
public record NhanVienDto(int Id, string HoTen);
public record MuiTiemDto(int Id, string NguoiTiem, string Vaccine, int MuiThu, string SoLo,
                         DateTime NgayGioTiem, string NhanVien);

public class MuiTiemService(IMuiTiemRepository repo, ILoVaccineRepository lots)
{
    public async Task<List<LichChoTiemDto>> LichChoTiemAsync()
        => (await repo.ListLichChoTiemAsync())
            .Select(x => new LichChoTiemDto(x.Id, x.NguoiTiem?.HoTen ?? "", x.VaccineId,
                                            x.Vaccine?.Ten ?? "", x.MuiThu, x.NgayHen)).ToList();

    public async Task<List<NhanVienDto>> NhanVienAsync()
        => (await repo.ListNhanVienAsync()).Select(x => new NhanVienDto(x.Id, x.HoTen)).ToList();

    public async Task<List<MuiTiemDto>> GanDayAsync()
        => (await repo.ListGanDayAsync(10)).Select(ToDto).ToList();

    public async Task<MuiTiemDto> GhiNhanAsync(GhiNhanMuiTiemRequest r)
    {
        var lich = await repo.GetLichAsync(r.LichTiemId)
                   ?? throw new DomainException("Lịch tiêm không tồn tại.");
        var lo = await lots.GetAsync(r.LoVaccineId)
                 ?? throw new DomainException("Lô vaccine không tồn tại.");
        var nv = await repo.GetNhanVienAsync(r.NhanVienId)
                 ?? throw new DomainException("Người ghi nhận phải là nhân viên y tế.");

        var coCcd = await repo.CoChongChiDinhDaXacNhanAsync(lich.NguoiTiemId, lich.VaccineId);
        var daTiemTruoc = lich.MuiThu <= 1
            || await repo.DaTiemMuiAsync(lich.NguoiTiemId, lich.VaccineId, lich.MuiThu - 1);

        var mui = MuiTiem.GhiNhan(lich, lo, nv.Id, coCcd, daTiemTruoc);  // quy tắc kiểm ở Domain
        await repo.LuuMuiTiemAsync(mui);                                   // lưu trong 1 transaction
        return ToDto((await repo.GetMuiAsync(mui.Id))!);
    }

    private static MuiTiemDto ToDto(MuiTiem m) => new(
        m.Id, m.LichTiem?.NguoiTiem?.HoTen ?? "", m.LichTiem?.Vaccine?.Ten ?? "",
        m.LichTiem?.MuiThu ?? 0, m.LoVaccine?.SoLo ?? "", m.NgayGioTiem, m.NhanVien?.HoTen ?? "");
}