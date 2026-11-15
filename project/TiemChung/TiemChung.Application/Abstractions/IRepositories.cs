using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface IVaccineRepository
{
    Task<List<Vaccine>> ListAsync();
    Task<bool> ExistsAsync(int id);
}

public interface ILoVaccineRepository
{
    Task<List<LoVaccine>> ListAsync();
    Task<LoVaccine?> GetAsync(int id);
    Task<bool> ExistsAsync(int vaccineId, string soLo);
    Task AddAsync(LoVaccine lo);
}