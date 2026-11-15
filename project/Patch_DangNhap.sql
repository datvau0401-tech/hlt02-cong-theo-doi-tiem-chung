/* PATCH: them dang nhap (ten dang nhap + mat khau) cho bang NguoiDung.
   Chay 1 lan trong SSMS tren CSDL TiemChungCongDong hien co (KHONG xoa du lieu). Chay lai nhieu lan van an toan.
   Mat khau duoc luu dang BAM (PBKDF2), khong luu mat khau that.

   Tai khoan demo:
     admin / Admin@123   (Quan tri 1,  vai tro QuanTri)
     bsc   / Bsc@123     (BS. Le C,    vai tro NhanVienYTe)
   Tai khoan vai tro NguoiDan khong duoc vao trang quan ly. */
USE TiemChungCongDong;
GO

IF COL_LENGTH('NguoiDung', 'TenDangNhap') IS NULL
    ALTER TABLE NguoiDung ADD TenDangNhap VARCHAR(50) NULL;
IF COL_LENGTH('NguoiDung', 'MatKhauHash') IS NULL
    ALTER TABLE NguoiDung ADD MatKhauHash VARCHAR(300) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_NguoiDung_TenDangNhap' AND object_id = OBJECT_ID('NguoiDung'))
    CREATE UNIQUE INDEX UX_NguoiDung_TenDangNhap ON NguoiDung (TenDangNhap) WHERE TenDangNhap IS NOT NULL;
GO

UPDATE NguoiDung
SET TenDangNhap = 'admin',
    MatKhauHash = 'PBKDF2$600000$nteU9ni4thncX9BxsmVTmw==$jITKSd5Xx2ImNgKKXIv3YHH6x0hLVWD3buDt8dtpbu8='
WHERE SubjectOidc = 'oidc-admin-1';

UPDATE NguoiDung
SET TenDangNhap = 'bsc',
    MatKhauHash = 'PBKDF2$600000$0+rrygv5vYAfpq+ekD+2ew==$0/wIvHQ6a/Su5R7QiuVOThAfPBjRUypfX4wovLCodbU='
WHERE SubjectOidc = 'oidc-nv-1';
GO

SELECT Id, HoTen, VaiTro, TenDangNhap,
       CASE WHEN MatKhauHash IS NULL THEN N'chua co mat khau' ELSE N'da dat mat khau' END AS MatKhau
FROM NguoiDung;
