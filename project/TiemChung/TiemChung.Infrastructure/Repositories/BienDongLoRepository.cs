using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class BienDongLoRepository(AppDbContext db) : IBienDongLoRepository
{
    public Task<List<BienDongLo>> ListGanDayAsync(int soLuong) =>
        Query().OrderByDescending(x => x.ThoiDiem).Take(soLuong).ToListAsync();

    public Task<BienDongLo?> GetAsync(long id) => Query().FirstOrDefaultAsync(x => x.Id == id);

    public async Task LuuAsync(BienDongLo bd)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        var loId = bd.LoVaccineId;
        var sl = bd.SoLuong;
        int dong;

        if (bd.Loai == LoaiBienDong.NhapThem)
        {
            // Nhập thêm: tăng cả tổng nhập lẫn số còn
            dong = await db.LoVaccines.Where(x => x.Id == loId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.SoLuongNhap, x => x.SoLuongNhap + sl)
                    .SetProperty(x => x.SoLuongCon, x => x.SoLuongCon + sl));
        }
        else
        {
            // Giảm bằng 1 câu UPDATE có điều kiện: không bao giờ làm tồn âm,
            // kể cả khi cùng lúc có người tiêm hoặc người khác điều chỉnh
            dong = await db.LoVaccines.Where(x => x.Id == loId && x.SoLuongCon >= sl)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.SoLuongCon, x => x.SoLuongCon - sl));
        }

        if (dong == 0)
            throw new DomainException("Số lượng tồn của lô vừa thay đổi, vui lòng tải lại và thử lại.");

        db.BienDongLos.Add(bd);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private IQueryable<BienDongLo> Query() =>
        db.BienDongLos.Include(x => x.NguoiThucHien)
          .Include(x => x.LoVaccine).ThenInclude(l => l!.Vaccine);
}