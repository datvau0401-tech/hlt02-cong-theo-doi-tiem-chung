using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class NhatKyRepository(AppDbContext db) : INhatKyRepository
{
    public Task<List<NhatKyHieuChinh>> ListAsync(string? tenBang, string? tuKhoa, int soLuong)
    {
        var q = db.NhatKyHieuChinhs.AsNoTracking().Include(x => x.NguoiSua).AsQueryable();

        if (!string.IsNullOrWhiteSpace(tenBang))
            q = q.Where(x => x.TenBang == tenBang);

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var k = tuKhoa.Trim();
            q = q.Where(x => x.MaBanGhi == k
                          || (x.LyDo != null && x.LyDo.Contains(k))
                          || (x.GiaTriCu != null && x.GiaTriCu.Contains(k))
                          || (x.GiaTriMoi != null && x.GiaTriMoi.Contains(k)));
        }

        return q.OrderByDescending(x => x.ThoiDiem).ThenByDescending(x => x.Id).Take(soLuong).ToListAsync();
    }
}
