namespace TiemChung.Domain.Entities;

public static class LoaiBienDong
{
    public const string NhapThem = "NhapThem";
    public const string Hong = "Hong";
    public const string HaoHut = "HaoHut";
    public const string Huy = "Huy";

    public static bool HopLe(string loai) =>
        loai is NhapThem or Hong or HaoHut or Huy;
}

public class BienDongLo
{
    public long Id { get; private set; }
    public int LoVaccineId { get; private set; }
    public LoVaccine? LoVaccine { get; private set; }
    public string Loai { get; private set; } = "";
    public int SoLuong { get; private set; }
    public DateTime ThoiDiem { get; private set; }
    public int NguoiThucHienId { get; private set; }
    public NguoiDung? NguoiThucHien { get; private set; }
    public string LyDo { get; private set; } = "";

    private BienDongLo() { }

    // Cách duy nhất để tạo một biến động: phải qua các kiểm tra bên dưới
    public static BienDongLo GhiNhan(LoVaccine lo, string loai, int soLuong,
                                     int nguoiThucHienId, string? lyDo)
    {
        if (!LoaiBienDong.HopLe(loai))
            throw new DomainException("Loại biến động không hợp lệ.");
        if (string.IsNullOrWhiteSpace(lyDo))
            throw new DomainException("Phải ghi lý do biến động.");

        if (loai == LoaiBienDong.NhapThem)
        {
            if (soLuong <= 0)
                throw new DomainException("Số lượng nhập thêm phải lớn hơn 0.");
            if (!lo.ConHanDung(DateOnly.FromDateTime(DateTime.Today)))
                throw new DomainException("Không thể nhập thêm vào lô đã hết hạn dùng.");
        }
        else
        {
            if (loai == LoaiBienDong.Huy)
            {
                if (lo.SoLuongCon <= 0)
                    throw new DomainException("Lô không còn liều nào để hủy.");
                soLuong = lo.SoLuongCon;          // hủy = hủy toàn bộ số còn lại
            }
            if (soLuong <= 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");
            if (soLuong > lo.SoLuongCon)
                throw new DomainException($"Lô chỉ còn {lo.SoLuongCon} liều, không thể giảm {soLuong}.");
        }

        return new BienDongLo
        {
            LoVaccineId = lo.Id,
            Loai = loai,
            SoLuong = soLuong,
            NguoiThucHienId = nguoiThucHienId,
            LyDo = lyDo.Trim(),
        };
    }
}