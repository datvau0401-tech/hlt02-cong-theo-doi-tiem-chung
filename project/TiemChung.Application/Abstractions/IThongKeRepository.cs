namespace TiemChung.Application.Abstractions;

public interface IThongKeRepository
{
    Task<int> DemNguoiTiemAsync();
    Task<int> DemLichChoTiemAsync();
    Task<int> DemLichChoTiemQuaHanAsync(DateOnly homNay);
    Task<int> DemMuiTiemTuAsync(DateTime tuLuc);
}
