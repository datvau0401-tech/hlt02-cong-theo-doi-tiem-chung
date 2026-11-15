using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class LoVaccineRepository(AppDbContext db) : ILoVaccineRepository
{
    private IQueryable<LoVaccine> Query() =>
        db.LoVaccines.Include(x => x.Vaccine).Include(x => x.NhaSanXuat);

    public Task<List<LoVaccine>> ListAsync() =>
        Query().AsNoTracking().OrderBy(x => x.HanDung).ToListAsync();

    public Task<LoVaccine?> GetAsync(int id) =>
        Query().FirstOrDefaultAsync(x => x.Id == id);

    public Task<bool> ExistsAsync(int vaccineId, string soLo) =>
        db.LoVaccines.AnyAsync(x => x.VaccineId == vaccineId && x.SoLo == soLo);

    public async Task AddAsync(LoVaccine lo)
    {
        db.LoVaccines.Add(lo);
        await db.SaveChangesAsync();
    }

    public Task<List<NhaSanXuat>> ListNhaSanXuatAsync() =>
        db.NhaSanXuats.AsNoTracking().OrderBy(x => x.Ten).ToListAsync();

    public Task<bool> NhaSanXuatExistsAsync(int id) =>
        db.NhaSanXuats.AnyAsync(x => x.Id == id);
}
