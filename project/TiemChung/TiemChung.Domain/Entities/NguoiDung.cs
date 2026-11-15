namespace TiemChung.Domain.Entities;

public class NguoiDung
{
    public int Id { get; private set; }
    public string HoTen { get; private set; } = "";
    public string VaiTro { get; private set; } = "";
    private NguoiDung() { }
}
