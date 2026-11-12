namespace TiemChung.Domain.Entities;

public class ChongChiDinh
{
    public int Id { get; private set; }
    public int NguoiTiemId { get; private set; }
    public int VaccineId { get; private set; }
    public string LyDo { get; private set; } = "";
    public string TrangThai { get; private set; } = "";   // ChuaXacNhan / DaXacNhan / DaThuHoi
    private ChongChiDinh() { }
}