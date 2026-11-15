namespace TiemChung.Domain.Entities;

public static class HanhDongNhatKy
{
    public const string Them = "THEM";
    public const string Sua = "SUA";
    public const string Huy = "HUY";
}

// Nhật ký hiệu chỉnh: chỉ thêm mới (CSDL có trigger chặn sửa/xóa)
public class NhatKyHieuChinh
{
    public long Id { get; private set; }
    public string TenBang { get; private set; } = "";
    public string MaBanGhi { get; private set; } = "";
    public string HanhDong { get; private set; } = "";
    public string? GiaTriCu { get; private set; }     // JSON
    public string? GiaTriMoi { get; private set; }    // JSON
    public int? NguoiSuaId { get; private set; }
    public NguoiDung? NguoiSua { get; private set; }
    public DateTime ThoiDiem { get; private set; }
    public string? LyDo { get; private set; }

    private NhatKyHieuChinh() { }

    public static NhatKyHieuChinh Tao(string tenBang, string maBanGhi, string hanhDong,
                                      string? giaTriCu, string? giaTriMoi, int? nguoiSuaId, string? lyDo)
    {
        if (hanhDong is not (HanhDongNhatKy.Them or HanhDongNhatKy.Sua or HanhDongNhatKy.Huy))
            throw new DomainException("Hành động nhật ký không hợp lệ.");

        return new NhatKyHieuChinh
        {
            TenBang = tenBang,
            MaBanGhi = maBanGhi,
            HanhDong = hanhDong,
            GiaTriCu = giaTriCu,
            GiaTriMoi = giaTriMoi,
            NguoiSuaId = nguoiSuaId,
            LyDo = string.IsNullOrWhiteSpace(lyDo) ? null : lyDo.Trim(),
        };
    }
}
