using TiemChung.Application.Abstractions;

namespace TiemChung.Application.Services;

public record LoCanChuYDto(int Id, string SoLo, string Vaccine, DateOnly HanDung,
                           int SoLuongCon, string Muc, string GhiChu);
public record DashboardDto(int SoLo, int TongTon, int LoSapHetHan, int LoHetHanConTon,
                           int SoNguoiTiem, int LichChoTiem, int LichQuaHen, int MuiTiemHomNay,
                           List<LoCanChuYDto> LoCanChuY);

public class DashboardService(ILoVaccineRepository lots, IThongKeRepository thongKe)
{
    private const int SoNgayCanhBao = 30;   // trùng với tham số SoNgayCanhBaoHetHan trong CSDL

    public async Task<DashboardDto> LayAsync()
    {
        var homNay = DateOnly.FromDateTime(DateTime.Today);
        var ds = await lots.ListAsync();

        var canChuY = new List<LoCanChuYDto>();
        int sapHet = 0, hetHanConTon = 0;

        foreach (var l in ds.Where(x => x.SoLuongCon > 0).OrderBy(x => x.HanDung))
        {
            var ngayConLai = l.HanDung.DayNumber - homNay.DayNumber;
            var ten = l.Vaccine?.Ten ?? "";
            if (ngayConLai < 0)
            {
                hetHanConTon++;
                canChuY.Add(new LoCanChuYDto(l.Id, l.SoLo, ten, l.HanDung, l.SoLuongCon, "bad",
                    $"Đã hết hạn {-ngayConLai} ngày, còn {l.SoLuongCon} liều cần xử lý"));
            }
            else if (ngayConLai <= SoNgayCanhBao)
            {
                sapHet++;
                canChuY.Add(new LoCanChuYDto(l.Id, l.SoLo, ten, l.HanDung, l.SoLuongCon, "warn",
                    $"Còn {ngayConLai} ngày là hết hạn"));
            }
        }

        return new DashboardDto(
            SoLo: ds.Count,
            TongTon: ds.Where(x => x.HanDung >= homNay).Sum(x => x.SoLuongCon),   // chỉ tính lô còn hạn
            LoSapHetHan: sapHet,
            LoHetHanConTon: hetHanConTon,
            SoNguoiTiem: await thongKe.DemNguoiTiemAsync(),
            LichChoTiem: await thongKe.DemLichChoTiemAsync(),
            LichQuaHen: await thongKe.DemLichChoTiemQuaHanAsync(homNay),
            MuiTiemHomNay: await thongKe.DemMuiTiemTuAsync(DateTime.Today),
            LoCanChuY: canChuY);
    }
}
