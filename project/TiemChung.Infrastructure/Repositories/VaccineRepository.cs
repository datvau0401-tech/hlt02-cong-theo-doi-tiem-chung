using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class VaccineRepository(AppDbContext db) : IVaccineRepository
{
    public Task<List<Vaccine>> ListAsync() => db.Vaccines.OrderBy(x => x.Ten).ToListAsync();

    public Task<bool> ExistsAsync(int id) => db.Vaccines.AnyAsync(x => x.Id == id);

    public Task<Vaccine?> GetAsync(int id) => db.Vaccines.FirstOrDefaultAsync(x => x.Id == id);

    public Task<bool> TenDaCoAsync(string ten, int? boQuaId) =>
        db.Vaccines.AnyAsync(x => x.Ten == ten && (boQuaId == null || x.Id != boQuaId));

    public async Task<int> MuiThuLonNhatDaDatAsync(int vaccineId) =>
        await db.LichTiems.Where(x => x.VaccineId == vaccineId && x.TrangThai != TrangThaiLich.DaHuy)
            .Select(x => (int?)x.MuiThu).MaxAsync() ?? 0;

    public async Task ThemAsync(Vaccine v, Func<Vaccine, NhatKyHieuChinh> nhatKy)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        db.Vaccines.Add(v);
        await db.SaveChangesAsync();                 // sau bước này v.Id đã có giá trị
        db.NhatKyHieuChinhs.Add(nhatKy(v));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task SuaAsync(Vaccine v, NhatKyHieuChinh nhatKy)
    {
        db.NhatKyHieuChinhs.Add(nhatKy);
        await db.SaveChangesAsync();                 // lưu phần đã sửa của v và nhật ký trong cùng 1 lần
    }
}
