namespace TiemChung.Domain.Entities;

public class NguoiTiem
{
    public int Id { get; private set; }
    public string HoTen { get; private set; } = "";
    private NguoiTiem() { }
}