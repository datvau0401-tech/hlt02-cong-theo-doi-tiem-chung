using TiemChung.Domain.Entities;

namespace TiemChung.Application.Abstractions;

public interface ITruyVetRepository
{
    Task<List<MuiTiem>> ListTheoLoAsync(int loVaccineId);
    Task<List<MuiTiem>> ListTheoNguoiTiemAsync(int nguoiTiemId);
}
