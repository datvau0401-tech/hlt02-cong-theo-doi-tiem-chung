namespace TiemChung.Domain.Entities;

public static class VaiTroNguoiDung
{
    public const string NguoiDan = "NguoiDan";
    public const string NhanVienYTe = "NhanVienYTe";
    public const string QuanTri = "QuanTri";
}

public class NguoiDung
{
    public int Id { get; private set; }
    public string SubjectOidc { get; private set; } = "";   // CSDL bắt buộc có; tài khoản tự tạo dùng "local:<tên đăng nhập>"
    public string HoTen { get; private set; } = "";
    public string VaiTro { get; private set; } = "";        // NguoiDan / NhanVienYTe / QuanTri
    public int? NguoiTiemId { get; private set; }           // chỉ có với tài khoản người dân
    public string? TenDangNhap { get; private set; }
    public string? MatKhauHash { get; private set; }        // PBKDF2, không lưu mật khẩu thật
    private NguoiDung() { }

    public static NguoiDung TaoNguoiDan(string? hoTen, string? tenDangNhap, string matKhauHash, int nguoiTiemId)
    {
        var u = new NguoiDung { VaiTro = VaiTroNguoiDung.NguoiDan, NguoiTiemId = nguoiTiemId };
        u.GanThongTin(hoTen, tenDangNhap, matKhauHash);
        return u;
    }

    public static NguoiDung TaoNhanSu(string? hoTen, string? vaiTro, string? tenDangNhap, string matKhauHash)
    {
        if (vaiTro is not (VaiTroNguoiDung.NhanVienYTe or VaiTroNguoiDung.QuanTri))
            throw new DomainException("Chỉ tạo được tài khoản nhân viên y tế hoặc quản trị viên.");
        var u = new NguoiDung { VaiTro = vaiTro };
        u.GanThongTin(hoTen, tenDangNhap, matKhauHash);
        return u;
    }

    // tenDangNhap = null thì giữ nguyên tên cũ (chỉ đổi mật khẩu)
    public void DatDangNhap(string? tenDangNhap, string matKhauHash)
    {
        var ten = tenDangNhap is null ? TenDangNhap : KiemTenDangNhap(tenDangNhap);
        if (ten is null) throw new DomainException("Tài khoản này chưa có tên đăng nhập, hãy nhập tên đăng nhập.");
        TenDangNhap = ten;
        SubjectOidc = SubjectOidc.StartsWith("local:") || SubjectOidc.Length == 0 ? "local:" + ten : SubjectOidc;
        MatKhauHash = matKhauHash;
    }

    private void GanThongTin(string? hoTen, string? tenDangNhap, string matKhauHash)
    {
        hoTen = (hoTen ?? "").Trim();
        if (hoTen.Length == 0) throw new DomainException("Họ tên không được để trống.");
        if (hoTen.Length > 100) throw new DomainException("Họ tên tối đa 100 ký tự.");
        var ten = KiemTenDangNhap(tenDangNhap);
        HoTen = hoTen; TenDangNhap = ten; MatKhauHash = matKhauHash; SubjectOidc = "local:" + ten;
    }

    public static string KiemTenDangNhap(string? ten)
    {
        ten = (ten ?? "").Trim();
        if (ten.Length is < 3 or > 50 || !ten.All(c => char.IsAsciiLetterOrDigit(c) || c is '.' or '_' or '-'))
            throw new DomainException("Tên đăng nhập gồm 3-50 ký tự: chữ không dấu, số, dấu chấm, gạch dưới hoặc gạch ngang.");
        return ten;
    }
}
