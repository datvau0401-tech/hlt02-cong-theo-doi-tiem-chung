using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class VaccineRepository(AppDbContext db) : IVaccineRepository
{
    public Task<List<Vaccine>> ListAsync() => db.Vaccines.OrderBy(x => x.Ten).ToListAsync();
    public Task<bool> ExistsAsync(int id) => db.Vaccines.AnyAsync(x => x.Id == id);
}