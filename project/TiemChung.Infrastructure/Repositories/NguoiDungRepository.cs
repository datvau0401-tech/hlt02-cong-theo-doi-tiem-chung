using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class NguoiDungRepository(AppDbContext db) : INguoiDungRepository
{
    public Task<NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap) =>
        db.NguoiDungs.AsNoTracking().FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);

    public Task<NguoiDung?> GetAsync(int id) => db.NguoiDungs.FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<NguoiDung>> ListAsync() =>
        db.NguoiDungs.AsNoTracking().OrderBy(x => x.VaiTro).ThenBy(x => x.HoTen).ToListAsync();

    public Task<bool> TenDangNhapDaCoAsync(string tenDangNhap, int? boQuaId) =>
        db.NguoiDungs.AnyAsync(x => x.TenDangNhap == tenDangNhap && (boQuaId == null || x.Id != boQuaId));

    public async Task<NguoiDung> DangKyNguoiDanAsync(NguoiTiem nguoiTiem, Func<NguoiTiem, NguoiDung> taoTaiKhoan,
                                                     Func<NguoiTiem, NguoiDung, NhatKyHieuChinh[]> nhatKy)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        db.NguoiTiems.Add(nguoiTiem);
        await db.SaveChangesAsync();                 // sau bước này nguoiTiem.Id đã có giá trị
        var u = taoTaiKhoan(nguoiTiem);
        db.NguoiDungs.Add(u);
        await db.SaveChangesAsync();
        db.NhatKyHieuChinhs.AddRange(nhatKy(nguoiTiem, u));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return u;
    }

    public async Task ThemAsync(NguoiDung u, Func<NguoiDung, NhatKyHieuChinh> nhatKy)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        db.NguoiDungs.Add(u);
        await db.SaveChangesAsync();
        db.NhatKyHieuChinhs.Add(nhatKy(u));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task SuaAsync(NguoiDung u, NhatKyHieuChinh nhatKy)
    {
        db.NhatKyHieuChinhs.Add(nhatKy);
        await db.SaveChangesAsync();
    }
}
