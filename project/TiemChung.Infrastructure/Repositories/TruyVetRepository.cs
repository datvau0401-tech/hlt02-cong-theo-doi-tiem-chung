using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class TruyVetRepository(AppDbContext db) : ITruyVetRepository
{
    public Task<List<MuiTiem>> ListTheoLoAsync(int loVaccineId) =>
        Query().Where(m => m.LoVaccineId == loVaccineId)
               .OrderByDescending(m => m.NgayGioTiem).ToListAsync();

    public Task<List<MuiTiem>> ListTheoNguoiTiemAsync(int nguoiTiemId) =>
        Query().Where(m => m.LichTiem!.NguoiTiemId == nguoiTiemId)
               .OrderByDescending(m => m.NgayGioTiem).ToListAsync();

    private IQueryable<MuiTiem> Query() =>
        db.MuiTiems.AsNoTracking()
          .Include(x => x.LoVaccine).ThenInclude(l => l!.NhaSanXuat).Include(x => x.NhanVien)
          .Include(x => x.LichTiem).ThenInclude(l => l!.NguoiTiem)
          .Include(x => x.LichTiem).ThenInclude(l => l!.Vaccine);
}
