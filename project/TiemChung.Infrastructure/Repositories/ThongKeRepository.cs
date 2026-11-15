using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class ThongKeRepository(AppDbContext db) : IThongKeRepository
{
    public Task<int> DemNguoiTiemAsync() => db.NguoiTiems.CountAsync();

    public Task<int> DemLichChoTiemAsync() =>
        db.LichTiems.CountAsync(x => x.TrangThai == TrangThaiLich.ChoTiem);

    public Task<int> DemLichChoTiemQuaHanAsync(DateOnly homNay) =>
        db.LichTiems.CountAsync(x => x.TrangThai == TrangThaiLich.ChoTiem && x.NgayHen < homNay);

    public Task<int> DemMuiTiemTuAsync(DateTime tuLuc) =>
        db.MuiTiems.CountAsync(x => x.NgayGioTiem >= tuLuc);
}
