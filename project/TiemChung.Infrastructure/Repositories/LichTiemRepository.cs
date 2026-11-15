using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class LichTiemRepository(AppDbContext db) : ILichTiemRepository
{
    public Task<List<LichTiem>> ListAsync(string? trangThai)
    {
        var q = db.LichTiems.AsNoTracking()
            .Include(x => x.NguoiTiem).Include(x => x.Vaccine)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(trangThai))
            q = q.Where(x => x.TrangThai == trangThai);
        return q.OrderByDescending(x => x.NgayHen).ThenByDescending(x => x.Id).Take(500).ToListAsync();
    }

    public Task<List<LichTiem>> ListTheoNguoiTiemAsync(int nguoiTiemId) =>
        db.LichTiems.AsNoTracking().Include(x => x.NguoiTiem).Include(x => x.Vaccine)
          .Where(x => x.NguoiTiemId == nguoiTiemId)
          .OrderByDescending(x => x.NgayHen).ThenByDescending(x => x.Id).ToListAsync();

    public Task<LichTiem?> GetAsync(int id) =>
        db.LichTiems.Include(x => x.NguoiTiem).Include(x => x.Vaccine)
          .FirstOrDefaultAsync(x => x.Id == id);

    public Task<bool> TonTaiLichConHieuLucAsync(int nguoiTiemId, int vaccineId, int muiThu) =>
        db.LichTiems.AnyAsync(x => x.NguoiTiemId == nguoiTiemId && x.VaccineId == vaccineId
                                && x.MuiThu == muiThu && x.TrangThai != TrangThaiLich.DaHuy);

    public async Task<DateOnly?> NgayMocMuiTruocAsync(int nguoiTiemId, int vaccineId, int muiTruoc)
    {
        var lich = await db.LichTiems.AsNoTracking()
            .Where(x => x.NguoiTiemId == nguoiTiemId && x.VaccineId == vaccineId
                     && x.MuiThu == muiTruoc && x.TrangThai != TrangThaiLich.DaHuy)
            .FirstOrDefaultAsync();
        if (lich is null) return null;

        if (lich.TrangThai == TrangThaiLich.DaTiem)
        {
            var ngayTiem = await db.MuiTiems.AsNoTracking()
                .Where(m => m.LichTiemId == lich.Id)
                .Select(m => (DateTime?)m.NgayGioTiem)
                .FirstOrDefaultAsync();
            if (ngayTiem is not null) return DateOnly.FromDateTime(ngayTiem.Value);
        }
        return lich.NgayHen;
    }

    public Task<bool> CoChongChiDinhDaXacNhanAsync(int nguoiTiemId, int vaccineId) =>
        db.ChongChiDinhs.AnyAsync(x => x.NguoiTiemId == nguoiTiemId
                                    && x.VaccineId == vaccineId && x.TrangThai == "DaXacNhan");

    public async Task ThemAsync(LichTiem l, Func<LichTiem, NhatKyHieuChinh> nhatKy)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        db.LichTiems.Add(l);
        await db.SaveChangesAsync();                 // sau bước này l.Id đã có giá trị
        db.NhatKyHieuChinhs.Add(nhatKy(l));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task SuaAsync(LichTiem l, NhatKyHieuChinh nhatKy)
    {
        db.NhatKyHieuChinhs.Add(nhatKy);
        await db.SaveChangesAsync();
    }
}
