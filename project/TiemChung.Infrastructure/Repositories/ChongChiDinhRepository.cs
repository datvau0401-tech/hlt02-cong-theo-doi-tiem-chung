using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class ChongChiDinhRepository(AppDbContext db) : IChongChiDinhRepository
{
    private IQueryable<ChongChiDinh> Query() =>
        db.ChongChiDinhs.Include(x => x.NguoiTiem).Include(x => x.Vaccine).Include(x => x.NguoiXacNhan);

    public Task<List<ChongChiDinh>> ListAsync(string? trangThai)
    {
        var q = Query().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(trangThai))
            q = q.Where(x => x.TrangThai == trangThai);
        return q.OrderByDescending(x => x.NgayGhiNhan).ThenByDescending(x => x.Id).Take(500).ToListAsync();
    }

    public Task<List<ChongChiDinh>> ListTheoNguoiTiemAsync(int nguoiTiemId) =>
        Query().AsNoTracking().Where(x => x.NguoiTiemId == nguoiTiemId)
               .OrderByDescending(x => x.NgayGhiNhan).ToListAsync();

    public Task<ChongChiDinh?> GetAsync(int id) => Query().FirstOrDefaultAsync(x => x.Id == id);

    public Task<bool> TonTaiConHieuLucAsync(int nguoiTiemId, int vaccineId) =>
        db.ChongChiDinhs.AnyAsync(x => x.NguoiTiemId == nguoiTiemId && x.VaccineId == vaccineId
                                    && x.TrangThai != TrangThaiCcd.DaThuHoi);

    public async Task ThemAsync(ChongChiDinh c, Func<ChongChiDinh, NhatKyHieuChinh> nhatKy)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        db.ChongChiDinhs.Add(c);
        await db.SaveChangesAsync();                 // sau bước này c.Id đã có giá trị
        db.NhatKyHieuChinhs.Add(nhatKy(c));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task SuaAsync(ChongChiDinh c, NhatKyHieuChinh nhatKy)
    {
        db.NhatKyHieuChinhs.Add(nhatKy);
        await db.SaveChangesAsync();
    }
}
