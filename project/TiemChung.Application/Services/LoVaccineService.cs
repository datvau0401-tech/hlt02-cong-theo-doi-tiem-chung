using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

// Dữ liệu trang web gửi lên khi nhập lô
public record CreateLoVaccineRequest(int VaccineId, string SoLo, DateOnly HanDung, int SoLuongNhap);

// Dữ liệu trả về cho trang web
public record LoVaccineDto(int Id, string SoLo, int VaccineId, string Vaccine,
                           DateOnly HanDung, int SoLuongNhap, int SoLuongCon);

public class LoVaccineService(ILoVaccineRepository lots, IVaccineRepository vaccines)
{
    public async Task<List<LoVaccineDto>> ListAsync()
        => (await lots.ListAsync()).Select(ToDto).ToList();

    public async Task<LoVaccineDto> CreateAsync(CreateLoVaccineRequest r)
    {
        if (!await vaccines.ExistsAsync(r.VaccineId))
            throw new DomainException("Vaccine không tồn tại.");

        var soLo = (r.SoLo ?? "").Trim();
        if (await lots.ExistsAsync(r.VaccineId, soLo))
            throw new DomainException("Số lô này đã tồn tại cho vaccine đã chọn.");

        var lo = LoVaccine.NhapMoi(r.VaccineId, soLo, r.HanDung, r.SoLuongNhap);
        await lots.AddAsync(lo);
        return ToDto((await lots.GetAsync(lo.Id))!);
    }

    private static LoVaccineDto ToDto(LoVaccine x) =>
        new(x.Id, x.SoLo, x.VaccineId, x.Vaccine?.Ten ?? "", x.HanDung, x.SoLuongNhap, x.SoLuongCon);
}