/* ============================================================
   NAP DU LIEU DEMO - TiemChungCongDong
   Chay 1 LAN trong SSMS (chon CSDL TiemChungCongDong, bam F5 ca file). Chay lai se tu bo qua.
   Yeu cau: da chay Patch_DangNhap.sql va script tao CSDL v2 + BienDongLo_Patch.sql.
   Du lieu nap: 5 nguoi tiem, lich + mui tiem, chong chi dinh, bien dong lo, nhat ky hieu chinh,
                va 2 tai khoan nguoi dan de demo dang nhap.
   Tai khoan nguoi dan:  hoa / Demo@1234   va   nguyenvana / Demo@1234
   ============================================================ */
USE TiemChungCongDong;
GO
SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM NguoiTiem WHERE SoDienThoai = '0911000001')
BEGIN
    PRINT N'Dữ liệu demo đã được nạp trước đó, không nạp lại.';
    RETURN;
END;

BEGIN TRY
BEGIN TRAN;

DECLARE @bs INT = (SELECT TOP 1 Id FROM NguoiDung WHERE VaiTro = 'NhanVienYTe' ORDER BY Id);
DECLARE @ad INT = (SELECT TOP 1 Id FROM NguoiDung WHERE VaiTro = 'QuanTri' ORDER BY Id);
IF @bs IS NULL OR @ad IS NULL THROW 50001, N'Thiếu tài khoản bác sĩ hoặc quản trị viên trong bảng NguoiDung.', 1;

DECLARE @hash VARCHAR(300) = 'PBKDF2$600000$nx/9lqk15UjTtWi20niu3Q==$ZYHySIqwKGJfcmxZVpAA516NLHlDz/t3wkOeuRgtX3w=';
DECLARE @today DATE = CAST(GETDATE() AS DATE);

/* ----- 1. Vaccine va lo (chi them neu chua co) ----- */
IF NOT EXISTS (SELECT 1 FROM Vaccine WHERE Ten = N'Vaccine cúm')
    INSERT INTO Vaccine (Ten, NhaSanXuat, SoMui, KhoangCachNgay) VALUES (N'Vaccine cúm', N'Sanofi', 1, NULL);
IF NOT EXISTS (SELECT 1 FROM Vaccine WHERE Ten = N'Vaccine viêm gan B')
    INSERT INTO Vaccine (Ten, NhaSanXuat, SoMui, KhoangCachNgay) VALUES (N'Vaccine viêm gan B', N'GSK', 3, 28);
IF NOT EXISTS (SELECT 1 FROM Vaccine WHERE Ten = N'Vaccine uốn ván')
    INSERT INTO Vaccine (Ten, NhaSanXuat, SoMui, KhoangCachNgay) VALUES (N'Vaccine uốn ván', N'Biological E', 2, 30);

IF NOT EXISTS (SELECT 1 FROM NhaSanXuat WHERE Ten = N'Sanofi Pasteur')
    INSERT INTO NhaSanXuat (Ten, QuocGia, DiaChi) VALUES (N'Sanofi Pasteur', N'Pháp', N'Lyon');
IF NOT EXISTS (SELECT 1 FROM NhaSanXuat WHERE Ten = N'GlaxoSmithKline Biologicals')
    INSERT INTO NhaSanXuat (Ten, QuocGia, DiaChi) VALUES (N'GlaxoSmithKline Biologicals', N'Bỉ', N'Wavre');
IF NOT EXISTS (SELECT 1 FROM NhaSanXuat WHERE Ten = N'Biological E. Limited')
    INSERT INTO NhaSanXuat (Ten, QuocGia, DiaChi) VALUES (N'Biological E. Limited', N'Ấn Độ', N'Hyderabad');
IF NOT EXISTS (SELECT 1 FROM NhaSanXuat WHERE Ten = N'VABIOTECH')
    INSERT INTO NhaSanXuat (Ten, QuocGia, DiaChi) VALUES (N'VABIOTECH', N'Việt Nam', N'Hà Nội');
DECLARE @sxSanofi INT = (SELECT Id FROM NhaSanXuat WHERE Ten = N'Sanofi Pasteur');
DECLARE @sxGsk    INT = (SELECT Id FROM NhaSanXuat WHERE Ten = N'GlaxoSmithKline Biologicals');
DECLARE @sxBioE   INT = (SELECT Id FROM NhaSanXuat WHERE Ten = N'Biological E. Limited');

DECLARE @vCum INT  = (SELECT Id FROM Vaccine WHERE Ten = N'Vaccine cúm');
DECLARE @vViem INT = (SELECT Id FROM Vaccine WHERE Ten = N'Vaccine viêm gan B');
DECLARE @vUon INT  = (SELECT Id FROM Vaccine WHERE Ten = N'Vaccine uốn ván');

IF NOT EXISTS (SELECT 1 FROM LoVaccine WHERE VaccineId = @vCum AND SoLo = 'VX-2409-A')
    INSERT INTO LoVaccine (VaccineId, NhaSanXuatId, SoLo, NgaySanXuat, HanDung, SoLuongNhap, SoLuongCon) VALUES (@vCum, @sxSanofi, 'VX-2409-A', '2026-09-12', '2027-10-12', 200, 200);
IF NOT EXISTS (SELECT 1 FROM LoVaccine WHERE VaccineId = @vViem AND SoLo = 'VX-2410-B')
    INSERT INTO LoVaccine (VaccineId, NhaSanXuatId, SoLo, NgaySanXuat, HanDung, SoLuongNhap, SoLuongCon) VALUES (@vViem, @sxGsk, 'VX-2410-B', '2026-10-01', '2027-06-30', 150, 150);
IF NOT EXISTS (SELECT 1 FROM LoVaccine WHERE VaccineId = @vUon AND SoLo = 'VX-2411-C')
    INSERT INTO LoVaccine (VaccineId, NhaSanXuatId, SoLo, NgaySanXuat, HanDung, SoLuongNhap, SoLuongCon) VALUES (@vUon, @sxBioE, 'VX-2411-C', '2026-09-15', '2027-03-15', 100, 100);

DECLARE @lCum INT  = (SELECT Id FROM LoVaccine WHERE VaccineId = @vCum  AND SoLo = 'VX-2409-A');
DECLARE @lViem INT = (SELECT Id FROM LoVaccine WHERE VaccineId = @vViem AND SoLo = 'VX-2410-B');
DECLARE @lUon INT  = (SELECT Id FROM LoVaccine WHERE VaccineId = @vUon  AND SoLo = 'VX-2411-C');

/* ----- 2. Bien dong lo: nhap them, hong, hao hut (nhap them tang ca tong nhap lan so con) ----- */
INSERT INTO BienDongLo (LoVaccineId, Loai, SoLuong, NguoiThucHienId, LyDo) VALUES
 (@lCum,  'NhapThem', 100, @bs, N'Nhập thêm đợt 2 (dữ liệu demo)'),
 (@lViem, 'NhapThem', 100, @bs, N'Nhập thêm đợt 2 (dữ liệu demo)'),
 (@lUon,  'NhapThem', 100, @bs, N'Nhập thêm đợt 2 (dữ liệu demo)'),
 (@lCum,  'Hong',       3, @bs, N'Vỡ lọ khi vận chuyển'),
 (@lViem, 'HaoHut',     2, @bs, N'Hao hụt khi chia liều');
UPDATE LoVaccine SET SoLuongNhap = SoLuongNhap + 100 WHERE Id IN (@lCum, @lViem, @lUon);

/* ----- 3. Nguoi tiem ----- */
INSERT INTO NguoiTiem (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email) VALUES
 (N'Nguyễn Thị Hoa',  '1992-03-14', 'Nu',  '0911000001', 'hoa@mail.com'),
 (N'Trần Văn Minh',   '1988-07-21', 'Nam', '0911000002', 'minh@mail.com'),
 (N'Lê Thị Lan',      '1995-11-02', 'Nu',  '0911000003', 'lan@mail.com'),
 (N'Phạm Quốc Huy',   '2000-01-30', 'Nam', '0911000004', 'huy@mail.com'),
 (N'Võ Thanh Tùng',   '1979-09-09', 'Nam', '0911000005', 'tung@mail.com');

DECLARE @hoa INT  = (SELECT Id FROM NguoiTiem WHERE SoDienThoai = '0911000001');
DECLARE @minh INT = (SELECT Id FROM NguoiTiem WHERE SoDienThoai = '0911000002');
DECLARE @lan INT  = (SELECT Id FROM NguoiTiem WHERE SoDienThoai = '0911000003');
DECLARE @huy INT  = (SELECT Id FROM NguoiTiem WHERE SoDienThoai = '0911000004');
DECLARE @tung INT = (SELECT Id FROM NguoiTiem WHERE SoDienThoai = '0911000005');

/* ----- 4. Lich tiem ----- */
INSERT INTO LichTiem (NguoiTiemId, VaccineId, MuiThu, NgayHen, TrangThai) VALUES
 (@hoa,  @vCum,  1, DATEADD(DAY, -12, @today), 'DaTiem'),
 (@minh, @vViem, 1, DATEADD(DAY, -60, @today), 'DaTiem'),
 (@minh, @vViem, 2, DATEADD(DAY, -30, @today), 'DaTiem'),
 (@minh, @vViem, 3, DATEADD(DAY,   5, @today), 'ChoTiem'),
 (@lan,  @vUon,  1, DATEADD(DAY, -10, @today), 'DaTiem'),
 (@lan,  @vUon,  2, DATEADD(DAY,  20, @today), 'ChoTiem'),
 (@huy,  @vCum,  1, DATEADD(DAY,  -3, @today), 'ChoTiem'),     -- qua han, chua tiem
 (@tung, @vViem, 1, DATEADD(DAY,  -5, @today), 'DaHuy'),
 (@tung, @vViem, 1, DATEADD(DAY,   7, @today), 'ChoTiem'),
 (@tung, @vCum,  1, @today,                    'DaTiem');       -- tiem hom nay

DECLARE @sHoa INT   = (SELECT Id FROM LichTiem WHERE NguoiTiemId = @hoa  AND VaccineId = @vCum  AND MuiThu = 1);
DECLARE @sMinh1 INT = (SELECT Id FROM LichTiem WHERE NguoiTiemId = @minh AND VaccineId = @vViem AND MuiThu = 1);
DECLARE @sMinh2 INT = (SELECT Id FROM LichTiem WHERE NguoiTiemId = @minh AND VaccineId = @vViem AND MuiThu = 2);
DECLARE @sMinh3 INT = (SELECT Id FROM LichTiem WHERE NguoiTiemId = @minh AND VaccineId = @vViem AND MuiThu = 3);
DECLARE @sLan1 INT  = (SELECT Id FROM LichTiem WHERE NguoiTiemId = @lan  AND VaccineId = @vUon  AND MuiThu = 1);
DECLARE @sTungHuy INT = (SELECT Id FROM LichTiem WHERE NguoiTiemId = @tung AND VaccineId = @vViem AND TrangThai = 'DaHuy');
DECLARE @sTungCum INT = (SELECT Id FROM LichTiem WHERE NguoiTiemId = @tung AND VaccineId = @vCum  AND MuiThu = 1);

/* ----- 5. Mui tiem da thuc hien (gan lo -> phuc vu truy vet) ----- */
INSERT INTO MuiTiem (LichTiemId, LoVaccineId, NgayGioTiem, NhanVienId) VALUES
 (@sHoa,   @lCum,  DATEADD(HOUR, 9,  CAST(DATEADD(DAY, -12, @today) AS DATETIME2)), @bs),
 (@sMinh1, @lViem, DATEADD(HOUR, 8,  CAST(DATEADD(DAY, -60, @today) AS DATETIME2)), @bs),
 (@sMinh2, @lViem, DATEADD(HOUR, 10, CAST(DATEADD(DAY, -30, @today) AS DATETIME2)), @bs),
 (@sLan1,  @lUon,  DATEADD(HOUR, 14, CAST(DATEADD(DAY, -10, @today) AS DATETIME2)), @bs),
 (@sTungCum, @lCum, SYSDATETIME(), @bs);

/* Tinh lai ton con cua moi lo = tong nhap - mui da tiem - hong/hao hut/huy  (de vw_DoiChieuTonLo luon khop) */
UPDATE lo SET SoLuongCon = lo.SoLuongNhap
       - (SELECT COUNT(*) FROM MuiTiem m WHERE m.LoVaccineId = lo.Id)
       - ISNULL((SELECT SUM(b.SoLuong) FROM BienDongLo b WHERE b.LoVaccineId = lo.Id AND b.Loai <> 'NhapThem'), 0)
FROM LoVaccine lo;

/* ----- 6. Chong chi dinh ----- */
INSERT INTO ChongChiDinh (NguoiTiemId, VaccineId, LyDo, TrangThai, NguoiXacNhanId, NgayXacNhan) VALUES
 (@huy, @vViem, N'Tiền sử dị ứng nặng với thành phần vaccine', 'DaXacNhan', @bs, SYSUTCDATETIME());
INSERT INTO ChongChiDinh (NguoiTiemId, VaccineId, LyDo, TrangThai) VALUES
 (@hoa, @vUon, N'Đang sốt cao, cần bác sĩ đánh giá', 'ChuaXacNhan');

/* ----- 7. Tai khoan nguoi dan de demo dang nhap ----- */
INSERT INTO NguoiDung (SubjectOidc, HoTen, VaiTro, NguoiTiemId, TenDangNhap, MatKhauHash)
VALUES ('local:hoa', N'Nguyễn Thị Hoa', 'NguoiDan', @hoa, 'hoa', @hash);
UPDATE NguoiDung SET TenDangNhap = 'nguyenvana', MatKhauHash = @hash
WHERE SubjectOidc = 'oidc-nd-1' AND TenDangNhap IS NULL
  AND NOT EXISTS (SELECT 1 FROM NguoiDung WHERE TenDangNhap = 'nguyenvana');

/* ----- 8. Nhat ky hieu chinh (de trang "Lich su hieu chinh" va khoi 10 co du lieu) ----- */
INSERT INTO NhatKyHieuChinh (TenBang, MaBanGhi, HanhDong, GiaTriCu, GiaTriMoi, NguoiSuaId, ThoiDiem, LyDo)
SELECT 'NguoiTiem', CAST(nt.Id AS VARCHAR(50)), 'THEM', NULL,
       (SELECT nt.HoTen AS hoTen, CONVERT(VARCHAR(10), nt.NgaySinh, 23) AS ngaySinh, nt.GioiTinh AS gioiTinh,
               nt.SoDienThoai AS soDienThoai, nt.Email AS email FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
       @ad, DATEADD(MINUTE, -60, SYSUTCDATETIME()), NULL
FROM NguoiTiem nt WHERE nt.Id IN (@hoa, @minh, @lan, @huy, @tung);

INSERT INTO NhatKyHieuChinh (TenBang, MaBanGhi, HanhDong, GiaTriCu, GiaTriMoi, NguoiSuaId, ThoiDiem, LyDo)
SELECT 'LichTiem', CAST(lt.Id AS VARCHAR(50)), 'THEM', NULL,
       (SELECT nt.HoTen AS nguoiTiem, v.Ten AS vaccine, lt.MuiThu AS muiThu,
               CONVERT(VARCHAR(10), lt.NgayHen, 23) AS ngayHen, 'ChoTiem' AS trangThai FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
       @bs, DATEADD(MINUTE, -30, SYSUTCDATETIME()), NULL
FROM LichTiem lt JOIN NguoiTiem nt ON nt.Id = lt.NguoiTiemId JOIN Vaccine v ON v.Id = lt.VaccineId
WHERE lt.NguoiTiemId IN (@hoa, @minh, @lan, @huy, @tung);

-- Doi ngay hen mui 3 cua Minh (SUA): cu +3 ngay -> moi +5 ngay
INSERT INTO NhatKyHieuChinh (TenBang, MaBanGhi, HanhDong, GiaTriCu, GiaTriMoi, NguoiSuaId, ThoiDiem, LyDo)
SELECT 'LichTiem', CAST(@sMinh3 AS VARCHAR(50)), 'SUA',
       (SELECT N'Trần Văn Minh' AS nguoiTiem, N'Vaccine viêm gan B' AS vaccine, 3 AS muiThu,
               CONVERT(VARCHAR(10), DATEADD(DAY, 3, @today), 23) AS ngayHen, 'ChoTiem' AS trangThai FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
       (SELECT N'Trần Văn Minh' AS nguoiTiem, N'Vaccine viêm gan B' AS vaccine, 3 AS muiThu,
               CONVERT(VARCHAR(10), DATEADD(DAY, 5, @today), 23) AS ngayHen, 'ChoTiem' AS trangThai FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
       @bs, DATEADD(MINUTE, -10, SYSUTCDATETIME()), NULL;

-- Huy lich cua Tung (HUY) co ly do
INSERT INTO NhatKyHieuChinh (TenBang, MaBanGhi, HanhDong, GiaTriCu, GiaTriMoi, NguoiSuaId, ThoiDiem, LyDo)
SELECT 'LichTiem', CAST(@sTungHuy AS VARCHAR(50)), 'HUY',
       (SELECT N'Võ Thanh Tùng' AS nguoiTiem, N'Vaccine viêm gan B' AS vaccine, 1 AS muiThu,
               CONVERT(VARCHAR(10), DATEADD(DAY, -5, @today), 23) AS ngayHen, 'ChoTiem' AS trangThai FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
       (SELECT N'Võ Thanh Tùng' AS nguoiTiem, N'Vaccine viêm gan B' AS vaccine, 1 AS muiThu,
               CONVERT(VARCHAR(10), DATEADD(DAY, -5, @today), 23) AS ngayHen, 'DaHuy' AS trangThai FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
       @bs, DATEADD(MINUTE, -5, SYSUTCDATETIME()), N'Bệnh, hẹn lại ngày khác';

COMMIT TRAN;
PRINT N'Đã nạp dữ liệu demo thành công.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    PRINT N'LỖI, đã hoàn tác, không nạp gì cả: ' + ERROR_MESSAGE();
END CATCH;
GO

/* Kiem tra nhanh sau khi nap */
SELECT * FROM vw_DoiChieuTonLo;                       -- cot Khop phai = 1 het
SELECT SoLo, HoTen, NgayGioTiem FROM vw_TruyVetTheoLo ORDER BY SoLo, NgayGioTiem;
GO
