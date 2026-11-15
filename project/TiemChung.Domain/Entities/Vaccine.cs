namespace TiemChung.Domain.Entities;

public class Vaccine
{
    public int Id { get; private set; }
    public string Ten { get; private set; } = "";
    public string? NhaSanXuat { get; private set; }
    public int SoMui { get; private set; }
    public int? KhoangCachNgay { get; private set; }

    private Vaccine() { }   // EF Core cần constructor rỗng để tạo đối tượng khi đọc từ CSDL

    public static Vaccine TaoMoi(string? ten, string? nhaSanXuat, int soMui, int? khoangCachNgay)
    {
        var v = new Vaccine();
        v.CapNhat(ten, nhaSanXuat, soMui, khoangCachNgay);
        return v;
    }

    public void CapNhat(string? ten, string? nhaSanXuat, int soMui, int? khoangCachNgay)
    {
        ten = (ten ?? "").Trim();
        if (ten.Length == 0) throw new DomainException("Tên vaccine không được để trống.");
        if (ten.Length > 100) throw new DomainException("Tên vaccine tối đa 100 ký tự.");

        nhaSanXuat = string.IsNullOrWhiteSpace(nhaSanXuat) ? null : nhaSanXuat.Trim();
        if (nhaSanXuat?.Length > 100) throw new DomainException("Nhà sản xuất tối đa 100 ký tự.");

        if (soMui is < 1 or > 10) throw new DomainException("Số mũi phải từ 1 đến 10.");
        if (soMui == 1) khoangCachNgay = null;      // chỉ 1 mũi thì không có khoảng cách
        else if (khoangCachNgay is null or < 0)
            throw new DomainException("Vaccine nhiều mũi phải có khoảng cách tối thiểu giữa 2 mũi (số ngày, từ 0 trở lên).");

        Ten = ten; NhaSanXuat = nhaSanXuat; SoMui = soMui; KhoangCachNgay = khoangCachNgay;
    }
}
