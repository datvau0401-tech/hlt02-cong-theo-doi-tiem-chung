using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class NguoiTiemRepository(AppDbContext db) : INguoiTiemRepository
{
    public Task<List<NguoiTiem>> ListAsync() =>
        db.NguoiTiems.OrderBy(x => x.HoTen).ThenBy(x => x.Id).Take(1000).ToListAsync();

    public Task<NguoiTiem?> GetAsync(int id) => db.NguoiTiems.FirstOrDefaultAsync(x => x.Id == id);

    public async Task ThemAsync(NguoiTiem n, Func<NguoiTiem, NhatKyHieuChinh> nhatKy)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        db.NguoiTiems.Add(n);
        await db.SaveChangesAsync();                 // sau bước này n.Id đã có giá trị
        db.NhatKyHieuChinhs.Add(nhatKy(n));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task SuaAsync(NguoiTiem n, NhatKyHieuChinh nhatKy)
    {
        db.NhatKyHieuChinhs.Add(nhatKy);
        await db.SaveChangesAsync();                 // lưu phần đã sửa của n và nhật ký trong cùng 1 lần
    }
}
