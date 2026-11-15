using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record VaccineRequest(string? Ten, string? NhaSanXuat, int SoMui, int? KhoangCachNgay);
public record VaccineDto(int Id, string Ten, string? NhaSanXuat, int SoMui, int? KhoangCachNgay);

public class VaccineService(IVaccineRepository repo)
{
    public async Task<List<VaccineDto>> ListAsync()
        => (await repo.ListAsync()).Select(ToDto).ToList();

    public async Task<VaccineDto> TaoAsync(VaccineRequest r, int? nguoiSuaId)
    {
        var v = Vaccine.TaoMoi(r.Ten, r.NhaSanXuat, r.SoMui, r.KhoangCachNgay);
        if (await repo.TenDaCoAsync(v.Ten, null))
            throw new DomainException("Đã có vaccine cùng tên.");

        await repo.ThemAsync(v, x => NhatKyHieuChinh.Tao(
            "Vaccine", x.Id.ToString(), HanhDongNhatKy.Them, null, NhatKyJson.Tao(Snap(x)), nguoiSuaId, null));
        return ToDto(v);
    }

    public async Task<VaccineDto> SuaAsync(int id, VaccineRequest r, int? nguoiSuaId)
    {
        var v = await repo.GetAsync(id) ?? throw new DomainException("Vaccine không tồn tại.");

        var cu = NhatKyJson.Tao(Snap(v));
        v.CapNhat(r.Ten, r.NhaSanXuat, r.SoMui, r.KhoangCachNgay);
        var moi = NhatKyJson.Tao(Snap(v));
        if (cu == moi) return ToDto(v);

        if (await repo.TenDaCoAsync(v.Ten, id))
            throw new DomainException("Đã có vaccine cùng tên.");
        var daDat = await repo.MuiThuLonNhatDaDatAsync(id);
        if (v.SoMui < daDat)
            throw new DomainException($"Đã có lịch tiêm mũi {daDat} của vaccine này nên số mũi không thể nhỏ hơn {daDat}.");

        await repo.SuaAsync(v, NhatKyHieuChinh.Tao(
            "Vaccine", id.ToString(), HanhDongNhatKy.Sua, cu, moi, nguoiSuaId, null));
        return ToDto(v);
    }

    private static object Snap(Vaccine v) => new { v.Ten, v.NhaSanXuat, v.SoMui, v.KhoangCachNgay };

    public static VaccineDto ToDto(Vaccine v) => new(v.Id, v.Ten, v.NhaSanXuat, v.SoMui, v.KhoangCachNgay);
}
