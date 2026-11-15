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

    // moc = ngày tiêm (hoặc ngày hẹn) của mũi trước; null nếu là mũi 1
    public static LichTiem Tao(Vaccine vaccine, int nguoiTiemId, int muiThu, DateOnly ngayHen, DateOnly homNay, DateOnly? moc)
    {
        if (muiThu < 1 || muiThu > vaccine.SoMui)
            throw new DomainException($"{vaccine.Ten} chỉ có {vaccine.SoMui} mũi.");
        KiemNgay(vaccine, muiThu, ngayHen, homNay, moc);
        return new LichTiem { NguoiTiemId = nguoiTiemId, VaccineId = vaccine.Id, MuiThu = muiThu, NgayHen = ngayHen };
    }

    public void DoiNgayHen(DateOnly ngayHen, DateOnly homNay, DateOnly? moc)
    {
        if (TrangThai != TrangThaiLich.ChoTiem)
            throw new DomainException("Chỉ đổi được ngày hẹn của lịch đang chờ tiêm.");
        KiemNgay(Vaccine, MuiThu, ngayHen, homNay, moc);
        NgayHen = ngayHen;
    }

    public void Huy()
    {
        if (TrangThai != TrangThaiLich.ChoTiem)
            throw new DomainException("Chỉ hủy được lịch đang chờ tiêm.");
        TrangThai = TrangThaiLich.DaHuy;
    }

    private static void KiemNgay(Vaccine? vaccine, int muiThu, DateOnly ngayHen, DateOnly homNay, DateOnly? moc)
    {
        if (ngayHen == default) throw new DomainException("Phải nhập ngày hẹn.");
        if (ngayHen < homNay) throw new DomainException("Ngày hẹn không được ở quá khứ.");
        if (muiThu > 1 && moc is not null && vaccine?.KhoangCachNgay is int cach && ngayHen < moc.Value.AddDays(cach))
            throw new DomainException($"Mũi {muiThu} phải cách mũi trước ít nhất {cach} ngày (từ {moc.Value.AddDays(cach):dd/MM/yyyy}).");
    }
}
