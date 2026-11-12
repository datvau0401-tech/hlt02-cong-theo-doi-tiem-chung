namespace TiemChung.Domain.Entities;

public class MuiTiem
{
    public int Id { get; private set; }
    public int LichTiemId { get; private set; }
    public LichTiem? LichTiem { get; private set; }
    public int LoVaccineId { get; private set; }
    public LoVaccine? LoVaccine { get; private set; }
    public DateTime NgayGioTiem { get; private set; }
    public int NhanVienId { get; private set; }
    public NguoiDung? NhanVien { get; private set; }

    private MuiTiem() { }

    // Cách duy nhất để tạo mũi tiêm: phải qua các kiểm tra bên dưới
    public static MuiTiem GhiNhan(LichTiem lich, LoVaccine lo, int nhanVienId,
                                  bool coChongChiDinhDaXacNhan, bool daTiemMuiTruoc)
    {
        if (lich.TrangThai != TrangThaiLich.ChoTiem)
            throw new DomainException("Lịch tiêm này không ở trạng thái chờ tiêm.");
        if (lo.VaccineId != lich.VaccineId)
            throw new DomainException("Lô vaccine không đúng loại vaccine của lịch tiêm.");
        if (!lo.ConHanDung(DateOnly.FromDateTime(DateTime.Today)))
            throw new DomainException("Lô vaccine đã hết hạn dùng.");
        if (lo.SoLuongCon <= 0)
            throw new DomainException("Lô vaccine đã hết số lượng.");
        if (coChongChiDinhDaXacNhan)
            throw new DomainException("Người tiêm có chống chỉ định đã xác nhận với vaccine này, không được tiêm.");
        if (lich.MuiThu > 1 && !daTiemMuiTruoc)
            throw new DomainException($"Chưa tiêm mũi {lich.MuiThu - 1} nên chưa thể tiêm mũi {lich.MuiThu}.");

        return new MuiTiem
        {
            LichTiemId = lich.Id,
            LoVaccineId = lo.Id,
            NhanVienId = nhanVienId,
            NgayGioTiem = DateTime.Now,
        };
    }
}