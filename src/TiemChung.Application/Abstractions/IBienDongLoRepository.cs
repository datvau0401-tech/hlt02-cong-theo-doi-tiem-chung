using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface IBienDongLoRepository
{
    Task<List<BienDongLo>> ListGanDayAsync(int soLuong);
    Task<BienDongLo?> GetAsync(long id);
    Task LuuAsync(BienDongLo bd);     // chỉnh tồn lô + thêm dòng biến động trong 1 transaction
}