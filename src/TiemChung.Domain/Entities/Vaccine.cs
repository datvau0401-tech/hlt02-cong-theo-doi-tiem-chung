namespace TiemChung.Domain.Entities;

public class Vaccine
{
    public int Id { get; private set; }
    public string Ten { get; private set; } = "";
    public string? NhaSanXuat { get; private set; }
    public int SoMui { get; private set; }
    public int? KhoangCachNgay { get; private set; }

    private Vaccine() { }   // EF Core cần constructor rỗng để tạo đối tượng khi đọc từ CSDL
}