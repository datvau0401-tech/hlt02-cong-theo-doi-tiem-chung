using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Application.Services;

public record NhatKyDto(long Id, string TenBang, string MaBanGhi, string HanhDong,
                        string? GiaTriCu, string? GiaTriMoi, string NguoiSua,
                        DateTime ThoiDiem, string? LyDo);

public class NhatKyService(INhatKyRepository repo)
{
    public async Task<List<NhatKyDto>> ListAsync(string? tenBang, string? tuKhoa, int soLuong)
        => (await repo.ListAsync(tenBang, tuKhoa, Math.Clamp(soLuong, 1, 500))).Select(ToDto).ToList();

    private static NhatKyDto ToDto(NhatKyHieuChinh n) => new(
        n.Id, n.TenBang, n.MaBanGhi, n.HanhDong, n.GiaTriCu, n.GiaTriMoi,
        n.NguoiSua?.HoTen ?? "(không rõ)", n.ThoiDiem, n.LyDo);
}
