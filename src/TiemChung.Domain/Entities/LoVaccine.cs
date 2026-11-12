namespace TiemChung.Domain.Entities;

public class LoVaccine
{
    public int Id { get; private set; }
    public int VaccineId { get; private set; }
    public Vaccine? Vaccine { get; private set; }
    public string SoLo { get; private set; } = "";
    public DateOnly HanDung { get; private set; }
    public int SoLuongNhap { get; private set; }
    public int SoLuongCon { get; private set; }
    public DateTime NgayNhap { get; private set; }

    private LoVaccine() { }   // dành cho EF Core

    // Cách duy nhất để tạo lô mới trong code: bắt buộc qua các kiểm tra bên dưới
    public static LoVaccine NhapMoi(int vaccineId, string soLo, DateOnly hanDung, int soLuongNhap)
    {
        if (string.IsNullOrWhiteSpace(soLo))
            throw new DomainException("Số lô không được để trống.");
        if (hanDung < DateOnly.FromDateTime(DateTime.Today))
            throw new DomainException("Không thể nhập lô đã hết hạn dùng.");
        if (soLuongNhap <= 0)
            throw new DomainException("Số lượng nhập phải lớn hơn 0.");

        return new LoVaccine
        {
            VaccineId = vaccineId,
            SoLo = soLo.Trim(),
            HanDung = hanDung,
            SoLuongNhap = soLuongNhap,
            SoLuongCon = soLuongNhap,   // vừa nhập thì còn nguyên số lượng
        };
    }

    public bool ConHanDung(DateOnly homNay) => HanDung >= homNay;

    // Trừ số lượng khi tiêm (sẽ dùng ở bước "Ghi nhận mũi tiêm")
    public void TruSoLuong(int soLuong = 1)
    {
        if (soLuong <= 0) throw new DomainException("Số lượng trừ phải lớn hơn 0.");
        if (soLuong > SoLuongCon) throw new DomainException("Lô không đủ số lượng.");
        SoLuongCon -= soLuong;
    }
}