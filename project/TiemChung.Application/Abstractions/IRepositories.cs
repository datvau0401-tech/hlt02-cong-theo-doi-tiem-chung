using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface IVaccineRepository
{
    Task<List<Vaccine>> ListAsync();
    Task<bool> ExistsAsync(int id);
    Task<Vaccine?> GetAsync(int id);                              // đối tượng được theo dõi để sửa
    Task<bool> TenDaCoAsync(string ten, int? boQuaId);
    Task<int> MuiThuLonNhatDaDatAsync(int vaccineId);             // mũi cao nhất trong các lịch còn hiệu lực
    // Lưu vaccine mới + ghi nhật ký trong 1 transaction (nhật ký cần Id vừa được tạo)
    Task ThemAsync(Vaccine v, Func<Vaccine, NhatKyHieuChinh> nhatKy);
    Task SuaAsync(Vaccine v, NhatKyHieuChinh nhatKy);
}

public interface ILoVaccineRepository
{
    Task<List<LoVaccine>> ListAsync();
    Task<LoVaccine?> GetAsync(int id);
    Task<bool> ExistsAsync(int vaccineId, string soLo);
    Task AddAsync(LoVaccine lo);
    Task<List<NhaSanXuat>> ListNhaSanXuatAsync();
    Task<bool> NhaSanXuatExistsAsync(int id);
}
