namespace TiemChung.Domain.Entities;

// Nhà sản xuất vaccine (nơi sản xuất): 1 nhà sản xuất - n lô vaccine
public class NhaSanXuat
{
    public int Id { get; private set; }
    public string Ten { get; private set; } = "";
    public string QuocGia { get; private set; } = "";
    public string? DiaChi { get; private set; }

    private NhaSanXuat() { }   // dành cho EF Core
}
