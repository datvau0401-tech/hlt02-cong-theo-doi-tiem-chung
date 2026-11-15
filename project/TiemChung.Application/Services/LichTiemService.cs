using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record TaoLichRequest(int NguoiTiemId, int VaccineId, int MuiThu, DateOnly NgayHen);
public record DoiNgayHenRequest(DateOnly NgayHen, string? LyDo);
public record HuyLichRequest(string? LyDo);
public record LichTiemDto(int Id, int NguoiTiemId, string NguoiTiem, int VaccineId, string Vaccine,
                          int MuiThu, DateOnly NgayHen, string TrangThai, string TenTrangThai);

public class LichTiemService(ILichTiemRepository repo, INguoiTiemRepository nguoiTiem, IVaccineRepository vaccines)
{
    private static DateOnly HomNay => DateOnly.FromDateTime(DateTime.Today);

    public async Task<List<LichTiemDto>> ListAsync(string? trangThai)
        => (await repo.ListAsync(trangThai)).Select(x => ToDto(x)).ToList();

    public async Task<List<LichTiemDto>> ListTheoNguoiTiemAsync(int nguoiTiemId)
        => (await repo.ListTheoNguoiTiemAsync(nguoiTiemId)).Select(x => ToDto(x)).ToList();

    // Người dân chỉ được hủy lịch của chính mình
    public async Task<LichTiemDto> HuyCuaNguoiTiemAsync(int nguoiTiemId, int id, string? lyDo, int? nguoiSuaId)
    {
        var l = await repo.GetAsync(id);
        if (l is null || l.NguoiTiemId != nguoiTiemId) throw new DomainException("Lịch tiêm không tồn tại.");
        return await HuyAsync(id, string.IsNullOrWhiteSpace(lyDo) ? "Người tiêm tự hủy lịch" : lyDo, nguoiSuaId);
    }

    public async Task<LichTiemDto> TaoAsync(TaoLichRequest r, int? nguoiSuaId)
    {
        var nt = await nguoiTiem.GetAsync(r.NguoiTiemId)
                 ?? throw new DomainException("Người tiêm không tồn tại.");
        var vc = (await vaccines.ListAsync()).FirstOrDefault(v => v.Id == r.VaccineId)
                 ?? throw new DomainException("Vaccine không tồn tại.");

        if (await repo.CoChongChiDinhDaXacNhanAsync(nt.Id, vc.Id))
            throw new DomainException("Người tiêm có chống chỉ định đã xác nhận với vaccine này, không thể đặt lịch.");
        if (await repo.TonTaiLichConHieuLucAsync(nt.Id, vc.Id, r.MuiThu))
            throw new DomainException($"Người này đã có lịch tiêm mũi {r.MuiThu} của {vc.Ten}.");

        // Mũi 2 trở đi: phải đã có lịch mũi trước, và đủ khoảng cách giữa 2 mũi
        DateOnly? moc = null;
        if (r.MuiThu > 1 && r.MuiThu <= vc.SoMui)
        {
            moc = await repo.NgayMocMuiTruocAsync(nt.Id, vc.Id, r.MuiThu - 1)
                  ?? throw new DomainException($"Chưa có lịch mũi {r.MuiThu - 1}, hãy đặt lịch mũi {r.MuiThu - 1} trước.");
        }

        var lich = LichTiem.Tao(vc, nt.Id, r.MuiThu, r.NgayHen, HomNay, moc);   // quy tắc kiểm ở Domain
        await repo.ThemAsync(lich, x => NhatKyHieuChinh.Tao(
            "LichTiem", x.Id.ToString(), HanhDongNhatKy.Them, null,
            NhatKyJson.Tao(Snap(x, nt.HoTen, vc.Ten)), nguoiSuaId, null));
        return ToDto(lich, nt.HoTen, vc.Ten);
    }

    public async Task<LichTiemDto> DoiNgayHenAsync(int id, DateOnly ngayHen, string? lyDo, int? nguoiSuaId)
    {
        if (string.IsNullOrWhiteSpace(lyDo))
            throw new DomainException("Phải ghi lý do đổi ngày hẹn.");

        var l = await repo.GetAsync(id) ?? throw new DomainException("Lịch tiêm không tồn tại.");

        DateOnly? moc = null;
        if (l.MuiThu > 1)
            moc = await repo.NgayMocMuiTruocAsync(l.NguoiTiemId, l.VaccineId, l.MuiThu - 1);

        var cu = NhatKyJson.Tao(Snap(l));
        l.DoiNgayHen(ngayHen, HomNay, moc);
        var moi = NhatKyJson.Tao(Snap(l));
        if (cu == moi) return ToDto(l);

        await repo.SuaAsync(l, NhatKyHieuChinh.Tao(
            "LichTiem", id.ToString(), HanhDongNhatKy.Sua, cu, moi, nguoiSuaId, lyDo.Trim()));
        return ToDto(l);
    }

    public async Task<LichTiemDto> HuyAsync(int id, string? lyDo, int? nguoiSuaId)
    {
        if (string.IsNullOrWhiteSpace(lyDo))
            throw new DomainException("Phải ghi lý do hủy lịch.");

        var l = await repo.GetAsync(id) ?? throw new DomainException("Lịch tiêm không tồn tại.");

        var cu = NhatKyJson.Tao(Snap(l));
        l.Huy();
        var moi = NhatKyJson.Tao(Snap(l));

        await repo.SuaAsync(l, NhatKyHieuChinh.Tao(
            "LichTiem", id.ToString(), HanhDongNhatKy.Huy, cu, moi, nguoiSuaId, lyDo));
        return ToDto(l);
    }

    private static object Snap(LichTiem l, string? nguoiTiem = null, string? vaccine = null) => new
    {
        NguoiTiem = nguoiTiem ?? l.NguoiTiem?.HoTen,
        Vaccine = vaccine ?? l.Vaccine?.Ten,
        l.MuiThu,
        l.NgayHen,
        l.TrangThai,
    };

    private static string TenTrangThai(string t) => t switch
    {
        TrangThaiLich.ChoTiem => "Chờ tiêm",
        TrangThaiLich.DaTiem => "Đã tiêm",
        TrangThaiLich.DaHuy => "Đã hủy",
        _ => t,
    };

    private static LichTiemDto ToDto(LichTiem l, string? nguoiTiem = null, string? vaccine = null) => new(
        l.Id, l.NguoiTiemId, nguoiTiem ?? l.NguoiTiem?.HoTen ?? "", l.VaccineId, vaccine ?? l.Vaccine?.Ten ?? "",
        l.MuiThu, l.NgayHen, l.TrangThai, TenTrangThai(l.TrangThai));
}
