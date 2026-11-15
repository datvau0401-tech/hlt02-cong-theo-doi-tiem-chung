using TiemChung.Application.Abstractions;
using TiemChung.Domain;

namespace TiemChung.Application.Services;

public record DatLichCuaToiRequest(int VaccineId, int MuiThu, DateOnly NgayHen);

// Việc người dân làm trên chính hồ sơ của mình (không đụng tới dữ liệu người khác)
public class CaNhanService(INguoiDungRepository users, INguoiTiemRepository people, LichTiemService lich,
                           TruyVetService truyVet, ChongChiDinhService ccd)
{
    private async Task<int> NguoiTiemIdAsync(int userId)
    {
        var u = await users.GetAsync(userId);
        return u?.NguoiTiemId
               ?? throw new DomainException("Tài khoản chưa liên kết hồ sơ người tiêm, hãy liên hệ quản trị viên.");
    }

    public async Task<NguoiTiemDto> ThongTinAsync(int userId)
    {
        var id = await NguoiTiemIdAsync(userId);
        var n = await people.GetAsync(id) ?? throw new DomainException("Hồ sơ người tiêm không tồn tại.");
        return NguoiTiemService.ToDto(n);
    }

    public async Task<List<LichTiemDto>> LichAsync(int userId)
        => await lich.ListTheoNguoiTiemAsync(await NguoiTiemIdAsync(userId));

    public async Task<LichTiemDto> DatLichAsync(int userId, DatLichCuaToiRequest r)
    {
        var id = await NguoiTiemIdAsync(userId);
        return await lich.TaoAsync(new TaoLichRequest(id, r.VaccineId, r.MuiThu, r.NgayHen), userId);
    }

    public async Task<LichTiemDto> HuyLichAsync(int userId, int lichId, string? lyDo)
        => await lich.HuyCuaNguoiTiemAsync(await NguoiTiemIdAsync(userId), lichId, lyDo, userId);

    public async Task<List<TruyVetDong>> MuiDaTiemAsync(int userId)
        => (await truyVet.TheoNguoiTiemAsync(await NguoiTiemIdAsync(userId))).DaTiem;

    public async Task<List<CcdDto>> ChongChiDinhAsync(int userId)
        => await ccd.ListTheoNguoiTiemAsync(await NguoiTiemIdAsync(userId));
}
