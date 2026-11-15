namespace TiemChung.Domain.Entities;

public static class TrangThaiCcd
{
    public const string ChuaXacNhan = "ChuaXacNhan";
    public const string DaXacNhan = "DaXacNhan";
    public const string DaThuHoi = "DaThuHoi";
}

// Chống chỉ định: người tiêm không được tiêm vaccine này (liên kết n-n NguoiTiem - Vaccine)
public class ChongChiDinh
{
    public int Id { get; private set; }
    public int NguoiTiemId { get; private set; }
    public NguoiTiem? NguoiTiem { get; private set; }
    public int VaccineId { get; private set; }
    public Vaccine? Vaccine { get; private set; }
    public string LyDo { get; private set; } = "";
    public string TrangThai { get; private set; } = TrangThaiCcd.ChuaXacNhan;
    public int? NguoiXacNhanId { get; private set; }
    public NguoiDung? NguoiXacNhan { get; private set; }
    public DateTime? NgayXacNhan { get; private set; }
    public DateTime NgayGhiNhan { get; private set; }
    private ChongChiDinh() { }

    public static ChongChiDinh GhiNhan(int nguoiTiemId, int vaccineId, string? lyDo)
        => new() { NguoiTiemId = nguoiTiemId, VaccineId = vaccineId, LyDo = KiemLyDo(lyDo) };

    // Chỉ sửa lý do khi chưa ai xác nhận
    public void SuaLyDo(string? lyDo)
    {
        if (TrangThai != TrangThaiCcd.ChuaXacNhan)
            throw new DomainException("Chỉ sửa được chống chỉ định chưa xác nhận.");
        LyDo = KiemLyDo(lyDo);
    }

    public void XacNhan(int? nguoiId)
    {
        if (TrangThai != TrangThaiCcd.ChuaXacNhan)
            throw new DomainException("Chống chỉ định này không ở trạng thái chưa xác nhận.");
        GhiNguoiXuLy(nguoiId);
        TrangThai = TrangThaiCcd.DaXacNhan;
    }

    public void ThuHoi(int? nguoiId)
    {
        if (TrangThai == TrangThaiCcd.DaThuHoi)
            throw new DomainException("Chống chỉ định này đã được thu hồi.");
        GhiNguoiXuLy(nguoiId);
        TrangThai = TrangThaiCcd.DaThuHoi;
    }

    // CSDL bắt buộc: đã xác nhận / thu hồi thì phải có người và thời điểm xử lý
    private void GhiNguoiXuLy(int? nguoiId)
    {
        NguoiXacNhanId = nguoiId ?? throw new DomainException("Không xác định được người thực hiện, hãy đăng nhập lại.");
        NgayXacNhan = DateTime.UtcNow;
    }

    private static string KiemLyDo(string? lyDo)
    {
        lyDo = (lyDo ?? "").Trim();
        if (lyDo.Length == 0) throw new DomainException("Phải ghi lý do chống chỉ định.");
        if (lyDo.Length > 300) throw new DomainException("Lý do tối đa 300 ký tự.");
        return lyDo;
    }
}
