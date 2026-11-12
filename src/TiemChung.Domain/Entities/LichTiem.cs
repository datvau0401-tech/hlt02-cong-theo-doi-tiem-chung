namespace TiemChung.Domain.Entities;

public static class TrangThaiLich
{
    public const string ChoTiem = "ChoTiem";
    public const string DaTiem = "DaTiem";
    public const string DaHuy = "DaHuy";
}

public class LichTiem
{
    public int Id { get; private set; }
    public int NguoiTiemId { get; private set; }
    public NguoiTiem? NguoiTiem { get; private set; }
    public int VaccineId { get; private set; }
    public Vaccine? Vaccine { get; private set; }
    public int MuiThu { get; private set; }
    public DateOnly NgayHen { get; private set; }
    public string TrangThai { get; private set; } = TrangThaiLich.ChoTiem;
    private LichTiem() { }
}