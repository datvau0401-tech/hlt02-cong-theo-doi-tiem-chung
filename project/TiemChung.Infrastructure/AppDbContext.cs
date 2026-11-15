using Microsoft.EntityFrameworkCore;
using TiemChung.Domain.Entities;

namespace TiemChung.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Vaccine> Vaccines => Set<Vaccine>();
    public DbSet<NhaSanXuat> NhaSanXuats => Set<NhaSanXuat>();
    public DbSet<LoVaccine> LoVaccines => Set<LoVaccine>();
    public DbSet<NguoiTiem> NguoiTiems => Set<NguoiTiem>();
    public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>();
    public DbSet<LichTiem> LichTiems => Set<LichTiem>();
    public DbSet<ChongChiDinh> ChongChiDinhs => Set<ChongChiDinh>();
    public DbSet<MuiTiem> MuiTiems => Set<MuiTiem>();
    public DbSet<BienDongLo> BienDongLos => Set<BienDongLo>();
    public DbSet<NhatKyHieuChinh> NhatKyHieuChinhs => Set<NhatKyHieuChinh>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Vaccine>().ToTable("Vaccine");
        b.Entity<NguoiTiem>().ToTable("NguoiTiem");
        b.Entity<NguoiDung>().ToTable("NguoiDung");

        b.Entity<ChongChiDinh>(e =>
        {
            e.ToTable("ChongChiDinh");
            e.Property(x => x.NgayGhiNhan).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasOne(x => x.NguoiTiem).WithMany().HasForeignKey(x => x.NguoiTiemId);
            e.HasOne(x => x.Vaccine).WithMany().HasForeignKey(x => x.VaccineId);
            e.HasOne(x => x.NguoiXacNhan).WithMany().HasForeignKey(x => x.NguoiXacNhanId);
        });

        b.Entity<LoVaccine>(e =>
        {
            e.ToTable("LoVaccine");
            e.Property(x => x.NgayNhap).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasOne(x => x.Vaccine).WithMany().HasForeignKey(x => x.VaccineId);
            e.HasOne(x => x.NhaSanXuat).WithMany().HasForeignKey(x => x.NhaSanXuatId);
        });

        b.Entity<NhaSanXuat>(e => e.ToTable("NhaSanXuat"));

        b.Entity<LichTiem>(e =>
        {
            e.ToTable("LichTiem");
            e.HasOne(x => x.NguoiTiem).WithMany().HasForeignKey(x => x.NguoiTiemId);
            e.HasOne(x => x.Vaccine).WithMany().HasForeignKey(x => x.VaccineId);
        });

        b.Entity<MuiTiem>(e =>
        {
            e.ToTable("MuiTiem");
            e.HasOne(x => x.LichTiem).WithMany().HasForeignKey(x => x.LichTiemId);
            e.HasOne(x => x.LoVaccine).WithMany().HasForeignKey(x => x.LoVaccineId);
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.NhanVienId);
        });

        b.Entity<BienDongLo>(e =>
        {
            e.ToTable("BienDongLo");
            e.Property(x => x.ThoiDiem).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasOne(x => x.LoVaccine).WithMany().HasForeignKey(x => x.LoVaccineId);
            e.HasOne(x => x.NguoiThucHien).WithMany().HasForeignKey(x => x.NguoiThucHienId);
        });

        b.Entity<NhatKyHieuChinh>(e =>
        {
            e.ToTable("NhatKyHieuChinh");
            e.Property(x => x.ThoiDiem).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasOne(x => x.NguoiSua).WithMany().HasForeignKey(x => x.NguoiSuaId);
        });
    }
}
