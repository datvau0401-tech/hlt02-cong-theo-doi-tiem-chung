using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record TruyVetDong(int MuiTiemId, int NguoiTiemId, string HoTen, string? SoDienThoai,
                          string Vaccine, int MuiThu, int LoVaccineId, string SoLo, DateOnly HanDungLo,
                          string NhaSanXuat, string QuocGia,
                          DateTime NgayGioTiem, string NhanVien);
public record TruyVetLoDto(int LoId, string SoLo, string Vaccine, string NhaSanXuat, string QuocGia, DateOnly? NgaySanXuat, DateOnly HanDung,
                           int SoLuongNhap, int SoLuongCon, List<TruyVetDong> DaTiem);
public record TruyVetNguoiDto(NguoiTiemDto NguoiTiem, List<TruyVetDong> DaTiem);

// Truy vết hai chiều: từ lô -> những ai đã tiêm, và từ người tiêm -> đã tiêm bằng lô nào
public class TruyVetService(ITruyVetRepository repo, ILoVaccineRepository lots, INguoiTiemRepository people)
{
    public async Task<TruyVetLoDto> TheoLoAsync(int loId)
    {
        var lo = await lots.GetAsync(loId) ?? throw new DomainException("Lô vaccine không tồn tại.");
        var ds = (await repo.ListTheoLoAsync(loId)).Select(ToDong).ToList();
        return new TruyVetLoDto(lo.Id, lo.SoLo, lo.Vaccine?.Ten ?? "", lo.NhaSanXuat?.Ten ?? "", lo.NhaSanXuat?.QuocGia ?? "",
                                lo.NgaySanXuat, lo.HanDung,
                                lo.SoLuongNhap, lo.SoLuongCon, ds);
    }

    public async Task<TruyVetNguoiDto> TheoNguoiTiemAsync(int nguoiTiemId)
    {
        var n = await people.GetAsync(nguoiTiemId) ?? throw new DomainException("Người tiêm không tồn tại.");
        var ds = (await repo.ListTheoNguoiTiemAsync(nguoiTiemId)).Select(ToDong).ToList();
        return new TruyVetNguoiDto(NguoiTiemService.ToDto(n), ds);
    }

    private static TruyVetDong ToDong(MuiTiem m) => new(
        m.Id,
        m.LichTiem?.NguoiTiemId ?? 0,
        m.LichTiem?.NguoiTiem?.HoTen ?? "",
        m.LichTiem?.NguoiTiem?.SoDienThoai,
        m.LichTiem?.Vaccine?.Ten ?? "",
        m.LichTiem?.MuiThu ?? 0,
        m.LoVaccineId,
        m.LoVaccine?.SoLo ?? "",
        m.LoVaccine?.HanDung ?? default,
        m.LoVaccine?.NhaSanXuat?.Ten ?? "",
        m.LoVaccine?.NhaSanXuat?.QuocGia ?? "",
        m.NgayGioTiem,
        m.NhanVien?.HoTen ?? "");
}
