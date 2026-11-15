using System.Security.Cryptography;

namespace TiemChung.Application.Services;

// Băm mật khẩu bằng PBKDF2-SHA256. Định dạng lưu: PBKDF2$<số vòng lặp>$<salt base64>$<hash base64>
public static class MatKhauHasher
{
    private const int SoVongLap = 600_000;
    private const int KichThuocSalt = 16;
    private const int KichThuocHash = 32;

    // Dùng để vẫn tốn thời gian băm khi tên đăng nhập không tồn tại (tránh lộ tài khoản nào có thật)
    private static readonly Lazy<string> HashGia = new(() => Hash("mat-khau-gia"));

    public static string Hash(string matKhau)
    {
        var salt = RandomNumberGenerator.GetBytes(KichThuocSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, SoVongLap, HashAlgorithmName.SHA256, KichThuocHash);
        return $"PBKDF2${SoVongLap}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string matKhau, string? daLuu)
    {
        var coThat = !string.IsNullOrEmpty(daLuu);
        var parts = (coThat ? daLuu! : HashGia.Value).Split('$');
        if (parts.Length != 4 || parts[0] != "PBKDF2" || !int.TryParse(parts[1], out var vong) || vong < 1)
            return false;

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var mongDoi = Convert.FromBase64String(parts[3]);
            var thucTe = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, vong, HashAlgorithmName.SHA256, mongDoi.Length);
            return CryptographicOperations.FixedTimeEquals(thucTe, mongDoi) && coThat;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
