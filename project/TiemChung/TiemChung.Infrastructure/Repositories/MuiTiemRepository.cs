using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Domain;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure.Repositories;

public class MuiTiemRepository(AppDbContext db) : IMuiTiemRepository
{
    public Task<List<LichTiem>> ListLichChoTiemAsync() =>
        db.LichTiems.Include(x => x.NguoiTiem).Include(x => x.Vaccine)
          .Where(x => x.TrangThai == TrangThaiLich.ChoTiem)
          .OrderBy(x => x.NgayHen).ToListAsync();

    public Task<LichTiem?> GetLichAsync(int id) =>
        db.LichTiems.Include(x => x.NguoiTiem).Include(x => x.Vaccine)
          .FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<NguoiDung>> ListNhanVienAsync() =>
        db.NguoiDungs.Where(x => x.VaiTro == "NhanVienYTe").OrderBy(x => x.HoTen).ToListAsync();

    public Task<NguoiDung?> GetNhanVienAsync(int id) =>
        db.NguoiDungs.FirstOrDefaultAsync(x => x.Id == id && x.VaiTro == "NhanVienYTe");

    public Task<bool> CoChongChiDinhDaXacNhanAsync(int nguoiTiemId, int vaccineId) =>
        db.ChongChiDinhs.AnyAsync(x => x.NguoiTiemId == nguoiTiemId
                                    && x.VaccineId == vaccineId && x.TrangThai == "DaXacNhan");

    public Task<bool> DaTiemMuiAsync(int nguoiTiemId, int vaccineId, int muiThu) =>
        db.LichTiems.AnyAsync(x => x.NguoiTiemId == nguoiTiemId && x.VaccineId == vaccineId
                                && x.MuiThu == muiThu && x.TrangThai == TrangThaiLich.DaTiem);

    // 3 thao tác trong 1 transaction: có lỗi ở bước nào thì hoàn tác tất cả
    public async Task LuuMuiTiemAsync(MuiTiem mui)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        // Trừ tồn bằng 1 câu UPDATE có điều kiện: 2 người cùng bấm thì chỉ 1 người trừ được lô cuối
        var tru = await db.LoVaccines
            .Where(x => x.Id == mui.LoVaccineId && x.SoLuongCon > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.SoLuongCon, x => x.SoLuongCon - 1));
        if (tru == 0) throw new DomainException("Lô vừa hết vaccine, vui lòng chọn lô khác.");

        // Chỉ đổi trạng thái nếu lịch vẫn đang chờ tiêm (chặn ghi nhận trùng)
        var doi = await db.LichTiems
            .Where(x => x.Id == mui.LichTiemId && x.TrangThai == TrangThaiLich.ChoTiem)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.TrangThai, TrangThaiLich.DaTiem));
        if (doi == 0) throw new DomainException("Lịch này vừa được ghi nhận rồi.");

        db.MuiTiems.Add(mui);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public Task<MuiTiem?> GetMuiAsync(int id) => Query().FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<MuiTiem>> ListGanDayAsync(int soLuong) =>
        Query().OrderByDescending(x => x.NgayGioTiem).Take(soLuong).ToListAsync();

    private IQueryable<MuiTiem> Query() =>
        db.MuiTiems.Include(x => x.LoVaccine).Include(x => x.NhanVien)
          .Include(x => x.LichTiem).ThenInclude(l => l!.NguoiTiem)
          .Include(x => x.LichTiem).ThenInclude(l => l!.Vaccine);
}