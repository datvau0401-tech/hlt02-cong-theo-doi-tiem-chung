using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

// Dữ liệu trang web gửi lên khi nhập lô
public record CreateLoVaccineRequest(int VaccineId, int NhaSanXuatId, string SoLo, DateOnly? NgaySanXuat, DateOnly HanDung, int SoLuongNhap);

// Dữ liệu trả về cho trang web
public record LoVaccineDto(int Id, string SoLo, int VaccineId, string Vaccine,
                           int NhaSanXuatId, string NhaSanXuat, string QuocGia, DateOnly? NgaySanXuat,
                           DateOnly HanDung, int SoLuongNhap, int SoLuongCon);
public record NhaSanXuatDto(int Id, string Ten, string QuocGia);

public class LoVaccineService(ILoVaccineRepository lots, IVaccineRepository vaccines)
{
    public async Task<List<LoVaccineDto>> ListAsync()
        => (await lots.ListAsync()).Select(ToDto).ToList();

    public async Task<LoVaccineDto> CreateAsync(CreateLoVaccineRequest r)
    {
        if (!await vaccines.ExistsAsync(r.VaccineId))
            throw new DomainException("Vaccine không tồn tại.");

        if (!await lots.NhaSanXuatExistsAsync(r.NhaSanXuatId))
            throw new DomainException("Nhà sản xuất không tồn tại.");

        var soLo = (r.SoLo ?? "").Trim();
        if (await lots.ExistsAsync(r.VaccineId, soLo))
            throw new DomainException("Số lô này đã tồn tại cho vaccine đã chọn.");

        var lo = LoVaccine.NhapMoi(r.VaccineId, r.NhaSanXuatId, soLo, r.NgaySanXuat, r.HanDung, r.SoLuongNhap);
        await lots.AddAsync(lo);
        return ToDto((await lots.GetAsync(lo.Id))!);
    }

    public async Task<List<NhaSanXuatDto>> ListNhaSanXuatAsync()
        => (await lots.ListNhaSanXuatAsync()).Select(x => new NhaSanXuatDto(x.Id, x.Ten, x.QuocGia)).ToList();

    private static LoVaccineDto ToDto(LoVaccine x) =>
        new(x.Id, x.SoLo, x.VaccineId, x.Vaccine?.Ten ?? "", x.NhaSanXuatId,
            x.NhaSanXuat?.Ten ?? "", x.NhaSanXuat?.QuocGia ?? "", x.NgaySanXuat,
            x.HanDung, x.SoLuongNhap, x.SoLuongCon);
}