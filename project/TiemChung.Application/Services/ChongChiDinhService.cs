using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record TaoCcdRequest(int NguoiTiemId, int VaccineId, string? LyDo);
public record SuaCcdRequest(string? LyDo);
public record ThuHoiCcdRequest(string? LyDo);
public record CcdDto(int Id, int NguoiTiemId, string NguoiTiem, int VaccineId, string Vaccine, string LyDo,
                     string TrangThai, string TenTrangThai, string? NguoiXacNhan,
                     DateTime? NgayXacNhan, DateTime NgayGhiNhan);

public class ChongChiDinhService(IChongChiDinhRepository repo, INguoiTiemRepository nguoiTiem, IVaccineRepository vaccines)
{
    public async Task<List<CcdDto>> ListAsync(string? trangThai)
        => (await repo.ListAsync(trangThai)).Select(ToDto).ToList();

    public async Task<List<CcdDto>> ListTheoNguoiTiemAsync(int nguoiTiemId)
        => (await repo.ListTheoNguoiTiemAsync(nguoiTiemId)).Select(ToDto).ToList();

    public async Task<CcdDto> TaoAsync(TaoCcdRequest r, int? nguoiSuaId)
    {
        var nt = await nguoiTiem.GetAsync(r.NguoiTiemId) ?? throw new DomainException("Người tiêm không tồn tại.");
        var vc = (await vaccines.ListAsync()).FirstOrDefault(v => v.Id == r.VaccineId)
                 ?? throw new DomainException("Vaccine không tồn tại.");
        if (await repo.TonTaiConHieuLucAsync(nt.Id, vc.Id))
            throw new DomainException("Người này đã có chống chỉ định còn hiệu lực với vaccine này.");

        var c = ChongChiDinh.GhiNhan(nt.Id, vc.Id, r.LyDo);
        await repo.ThemAsync(c, x => NhatKyHieuChinh.Tao(
            "ChongChiDinh", x.Id.ToString(), HanhDongNhatKy.Them, null,
            NhatKyJson.Tao(Snap(x, nt.HoTen, vc.Ten)), nguoiSuaId, null));
        return (await repo.GetAsync(c.Id)) is { } day ? ToDto(day) : ToDto(c);
    }

    public async Task<CcdDto> SuaLyDoAsync(int id, string? lyDo, int? nguoiSuaId)
    {
        var c = await repo.GetAsync(id) ?? throw new DomainException("Chống chỉ định không tồn tại.");
        var cu = NhatKyJson.Tao(Snap(c));
        c.SuaLyDo(lyDo);
        var moi = NhatKyJson.Tao(Snap(c));
        if (cu == moi) return ToDto(c);
        await repo.SuaAsync(c, NhatKyHieuChinh.Tao(
            "ChongChiDinh", id.ToString(), HanhDongNhatKy.Sua, cu, moi, nguoiSuaId, null));
        return ToDto(c);
    }

    public async Task<CcdDto> XacNhanAsync(int id, int? nguoiId)
    {
        var c = await repo.GetAsync(id) ?? throw new DomainException("Chống chỉ định không tồn tại.");
        var cu = NhatKyJson.Tao(Snap(c));
        c.XacNhan(nguoiId);
        await repo.SuaAsync(c, NhatKyHieuChinh.Tao(
            "ChongChiDinh", id.ToString(), HanhDongNhatKy.Sua, cu, NhatKyJson.Tao(Snap(c)), nguoiId, "Xác nhận chống chỉ định"));
        return ToDto(c);
    }

    public async Task<CcdDto> ThuHoiAsync(int id, string? lyDo, int? nguoiId)
    {
        if (string.IsNullOrWhiteSpace(lyDo))
            throw new DomainException("Phải ghi lý do thu hồi.");
        var c = await repo.GetAsync(id) ?? throw new DomainException("Chống chỉ định không tồn tại.");
        var cu = NhatKyJson.Tao(Snap(c));
        c.ThuHoi(nguoiId);
        await repo.SuaAsync(c, NhatKyHieuChinh.Tao(
            "ChongChiDinh", id.ToString(), HanhDongNhatKy.Huy, cu, NhatKyJson.Tao(Snap(c)), nguoiId, lyDo));
        return ToDto(c);
    }

    private static object Snap(ChongChiDinh c, string? nguoiTiem = null, string? vaccine = null) => new
    {
        NguoiTiem = nguoiTiem ?? c.NguoiTiem?.HoTen,
        Vaccine = vaccine ?? c.Vaccine?.Ten,
        c.LyDo,
        c.TrangThai,
    };

    private static string TenTrangThai(string t) => t switch
    {
        TrangThaiCcd.ChuaXacNhan => "Chưa xác nhận",
        TrangThaiCcd.DaXacNhan => "Đã xác nhận",
        TrangThaiCcd.DaThuHoi => "Đã thu hồi",
        _ => t,
    };

    private static CcdDto ToDto(ChongChiDinh c) => new(
        c.Id, c.NguoiTiemId, c.NguoiTiem?.HoTen ?? "", c.VaccineId, c.Vaccine?.Ten ?? "", c.LyDo,
        c.TrangThai, TenTrangThai(c.TrangThai), c.NguoiXacNhan?.HoTen, c.NgayXacNhan, c.NgayGhiNhan);
}
