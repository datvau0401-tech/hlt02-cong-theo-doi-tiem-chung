using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class LoVaccineRepository(AppDbContext db) : ILoVaccineRepository
{
    public Task<List<LoVaccine>> ListAsync() =>
        db.LoVaccines.Include(x => x.Vaccine).OrderByDescending(x => x.NgayNhap).ToListAsync();

    public Task<LoVaccine?> GetAsync(int id) =>
        db.LoVaccines.Include(x => x.Vaccine).FirstOrDefaultAsync(x => x.Id == id);

    public Task<bool> ExistsAsync(int vaccineId, string soLo) =>
        db.LoVaccines.AnyAsync(x => x.VaccineId == vaccineId && x.SoLo == soLo);

    public async Task AddAsync(LoVaccine lo)
    {
        db.LoVaccines.Add(lo);
        await db.SaveChangesAsync();
    }
}