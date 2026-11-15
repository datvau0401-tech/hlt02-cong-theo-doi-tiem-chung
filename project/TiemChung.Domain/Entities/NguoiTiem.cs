namespace TiemChung.Domain.Entities;

public class NguoiTiem
{
    public int Id { get; private set; }
    public string HoTen { get; private set; } = "";
    public DateOnly NgaySinh { get; private set; }
    public string? GioiTinh { get; private set; }      // Nam / Nu / Khac
    public string? SoDienThoai { get; private set; }
    public string? Email { get; private set; }
    public string? Cccd { get; private set; }          // căn cước công dân, 12 chữ số

    private NguoiTiem() { }

    public static NguoiTiem TaoMoi(string? hoTen, DateOnly ngaySinh, string? gioiTinh, string? soDienThoai, string? email, string? cccd = null)
    {
        var n = new NguoiTiem();
        n.CapNhat(hoTen, ngaySinh, gioiTinh, soDienThoai, email, cccd);
        return n;
    }

    // Cả tạo mới lẫn sửa đều đi qua đây nên chỉ cần kiểm một chỗ
    public void CapNhat(string? hoTen, DateOnly ngaySinh, string? gioiTinh, string? soDienThoai, string? email, string? cccd = null)
    {
        hoTen = (hoTen ?? "").Trim();
        if (hoTen.Length == 0) throw new DomainException("Họ tên không được để trống.");
        if (hoTen.Length > 100) throw new DomainException("Họ tên tối đa 100 ký tự.");

        if (ngaySinh == default) throw new DomainException("Phải nhập ngày sinh.");
        if (ngaySinh > DateOnly.FromDateTime(DateTime.Today)) throw new DomainException("Ngày sinh không được ở tương lai.");

        gioiTinh = string.IsNullOrWhiteSpace(gioiTinh) ? null : gioiTinh.Trim();
        if (gioiTinh is not (null or "Nam" or "Nu" or "Khac")) throw new DomainException("Giới tính không hợp lệ.");

        soDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai.Trim();
        if (soDienThoai is not null && (soDienThoai.Length is < 8 or > 15 || !soDienThoai.All(char.IsDigit)))
            throw new DomainException("Số điện thoại phải gồm 8-15 chữ số.");

        email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        if (email is not null && (email.Length > 100 || !email.Contains('@') || email.Contains(' ')))
            throw new DomainException("Email không hợp lệ.");

        cccd = string.IsNullOrWhiteSpace(cccd) ? null : cccd.Trim();
        if (cccd is not null && (cccd.Length != 12 || !cccd.All(char.IsDigit)))
            throw new DomainException("Số căn cước công dân phải gồm đúng 12 chữ số.");

        HoTen = hoTen; NgaySinh = ngaySinh; GioiTinh = gioiTinh; SoDienThoai = soDienThoai; Email = email; Cccd = cccd;
    }
}
