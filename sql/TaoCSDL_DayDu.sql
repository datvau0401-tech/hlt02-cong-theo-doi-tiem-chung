/* ============================================================
   TAO MOI TOAN BO CSDL TiemChungCongDong (1 FILE)  -  SQL Server
   Gom 3 script theo dung thu tu:
     PHAN 1: tao CSDL + bang + view + du lieu goc   (TiemChungCongDong_v2.sql)
     PHAN 2: bien dong lo                           (BienDongLo_Patch.sql)
     PHAN 3: dang nhap (ten dang nhap + mat khau)   (Patch_DangNhap.sql)
   Sau do chay tiep Seed_DemoData.sql neu muon co du lieu demo.
   CANH BAO: file nay XOA CSDL TiemChungCongDong neu da co roi tao lai tu dau.
   Tai khoan: admin / Admin@123   va   bsc / Bsc@123
   ============================================================ */


/* ################ PHAN 1: TAO CSDL ################ */
/* ============================================================
   CSDL: TiemChungCongDong  (SQL Server) - PHIEN BAN 2
   Sinh tu mo hinh quan he theo 9 quy tac anh xa cua thay:
     QT1 lop -> bang | QT2 lien ket 1-1 (FK + UNIQUE) | QT3 lien ket 1-n (FK ben n)
     QT4 lien ket n-n (bang trung gian: ChongChiDinh, LichTiem, MuiTiem)
     QT8 gia tri roi rac -> bang danh muc DM_* | QT9 tham so he thong -> ThamSoHeThong
   Chay toan bo trong SSMS (F5). Chay lai duoc nhieu lan (xoa va tao lai CSDL).
   LUU Y: script chua duoc chay thu tren SQL Server that - hay chay va bao loi neu co.
   ============================================================ */
USE master;
GO
IF DB_ID(N'TiemChungCongDong') IS NOT NULL
BEGIN
    ALTER DATABASE TiemChungCongDong SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TiemChungCongDong;
END
GO
CREATE DATABASE TiemChungCongDong COLLATE Vietnamese_CI_AS;
GO
USE TiemChungCongDong;
GO

/* ===== QT8: cac bang danh muc (gia tri roi rac) ===== */
CREATE TABLE DM_GioiTinh      (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);
CREATE TABLE DM_VaiTro        (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);
CREATE TABLE DM_TrangThaiLich (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);
CREATE TABLE DM_TrangThaiCCD  (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);
CREATE TABLE DM_KenhNhac      (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);
CREATE TABLE DM_TrangThaiNhac (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);
CREATE TABLE DM_HanhDong      (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);

INSERT INTO DM_GioiTinh VALUES ('Nam',N'Nam'),('Nu',N'Nữ'),('Khac',N'Khác');
INSERT INTO DM_VaiTro VALUES ('NguoiDan',N'Người dân'),('NhanVienYTe',N'Nhân viên y tế'),('QuanTri',N'Quản trị viên');
INSERT INTO DM_TrangThaiLich VALUES ('ChoTiem',N'Chờ tiêm'),('DaTiem',N'Đã tiêm'),('DaHuy',N'Đã hủy');
INSERT INTO DM_TrangThaiCCD VALUES ('ChuaXacNhan',N'Chưa xác nhận'),('DaXacNhan',N'Đã xác nhận'),('DaThuHoi',N'Đã thu hồi');
INSERT INTO DM_KenhNhac VALUES ('Email',N'Email'),('SMS',N'Tin nhắn SMS');
INSERT INTO DM_TrangThaiNhac VALUES ('ChoGui',N'Chờ gửi'),('DaGui',N'Đã gửi'),('Loi',N'Lỗi');
INSERT INTO DM_HanhDong VALUES ('THEM',N'Thêm'),('SUA',N'Sửa'),('HUY',N'Hủy');

/* ===== QT9: tham so he thong ===== */
CREATE TABLE ThamSoHeThong (
    TenThamSo VARCHAR(50)   PRIMARY KEY,
    GiaTri    NVARCHAR(100) NOT NULL,
    MoTa      NVARCHAR(200) NULL
);
INSERT INTO ThamSoHeThong VALUES
 ('SoNgayNhacTruoc',      N'3',  N'Nhắc lịch trước ngày hẹn bao nhiêu ngày'),
 ('SoNgayCanhBaoHetHan',  N'30', N'Cảnh báo lô còn dưới bao nhiêu ngày là hết hạn');

/* ===== QT1: cac lop entity -> bang ===== */
CREATE TABLE NguoiTiem (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    HoTen        NVARCHAR(100) NOT NULL,
    NgaySinh     DATE          NOT NULL,
    GioiTinh     VARCHAR(20)   NULL REFERENCES DM_GioiTinh(Ma),
    SoDienThoai  VARCHAR(15)   NULL,
    Email        VARCHAR(100)  NULL,
    CONSTRAINT CK_NguoiTiem_NgaySinh CHECK (NgaySinh <= CAST(GETDATE() AS DATE))
);

CREATE TABLE NguoiDung (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    SubjectOidc  VARCHAR(200)  NOT NULL UNIQUE,
    HoTen        NVARCHAR(100) NOT NULL,
    VaiTro       VARCHAR(20)   NOT NULL REFERENCES DM_VaiTro(Ma),
    NguoiTiemId  INT           NULL REFERENCES NguoiTiem(Id)   -- QT2: lien ket 1-1 (0..1)
);
-- QT2: FK + UNIQUE (chi rang buoc khi khac NULL)
CREATE UNIQUE INDEX UX_NguoiDung_NguoiTiem ON NguoiDung(NguoiTiemId) WHERE NguoiTiemId IS NOT NULL;

CREATE TABLE Vaccine (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Ten             NVARCHAR(100) NOT NULL UNIQUE,
    NhaSanXuat      NVARCHAR(100) NULL,
    SoMui           INT NOT NULL,
    KhoangCachNgay  INT NULL,
    CONSTRAINT CK_Vaccine_SoMui CHECK (SoMui >= 1),
    CONSTRAINT CK_Vaccine_KhoangCach CHECK (KhoangCachNgay IS NULL OR KhoangCachNgay >= 0)
);

/* Noi san xuat: 1 NhaSanXuat - n LoVaccine (FK o ben n). Noi san xuat gan theo LO de truy vet/thu hoi */
CREATE TABLE NhaSanXuat (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Ten      NVARCHAR(150) NOT NULL UNIQUE,
    QuocGia  NVARCHAR(60)  NOT NULL,
    DiaChi   NVARCHAR(250) NULL
);

/* QT3: 1 Vaccine - n LoVaccine (FK o ben n) */
CREATE TABLE LoVaccine (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    VaccineId    INT NOT NULL REFERENCES Vaccine(Id),
    NhaSanXuatId INT NOT NULL REFERENCES NhaSanXuat(Id),
    SoLo         VARCHAR(50) NOT NULL,
    NgaySanXuat  DATE NULL,
    HanDung      DATE NOT NULL,
    SoLuongNhap  INT NOT NULL,
    SoLuongCon   INT NOT NULL,      -- cot suy dien duoc luu co chu dich (xem bao cao muc 4)
    NgayNhap     DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_LoVaccine UNIQUE (VaccineId, SoLo),
    CONSTRAINT CK_Lo_NgaySanXuat CHECK (NgaySanXuat IS NULL OR NgaySanXuat < HanDung),
    CONSTRAINT CK_Lo_SoLuongNhap CHECK (SoLuongNhap > 0),
    CONSTRAINT CK_Lo_SoLuongCon  CHECK (SoLuongCon >= 0 AND SoLuongCon <= SoLuongNhap)
);

/* QT4: lien ket n-n NguoiTiem - Vaccine (co thuoc tinh) -> bang trung gian ChongChiDinh */
CREATE TABLE ChongChiDinh (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    NguoiTiemId    INT NOT NULL REFERENCES NguoiTiem(Id),
    VaccineId      INT NOT NULL REFERENCES Vaccine(Id),
    LyDo           NVARCHAR(300) NOT NULL,
    TrangThai      VARCHAR(20) NOT NULL DEFAULT 'ChuaXacNhan' REFERENCES DM_TrangThaiCCD(Ma),
    NguoiXacNhanId INT NULL REFERENCES NguoiDung(Id),
    NgayXacNhan    DATETIME2 NULL,
    NgayGhiNhan    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT CK_CCD_XacNhan CHECK (TrangThai = 'ChuaXacNhan' OR (NguoiXacNhanId IS NOT NULL AND NgayXacNhan IS NOT NULL))
);

/* QT4: lien ket n-n NguoiTiem - Vaccine -> bang trung gian LichTiem */
CREATE TABLE LichTiem (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    NguoiTiemId  INT NOT NULL REFERENCES NguoiTiem(Id),
    VaccineId    INT NOT NULL REFERENCES Vaccine(Id),
    MuiThu       INT NOT NULL,
    NgayHen      DATE NOT NULL,
    TrangThai    VARCHAR(20) NOT NULL DEFAULT 'ChoTiem' REFERENCES DM_TrangThaiLich(Ma),
    CONSTRAINT CK_Lich_MuiThu CHECK (MuiThu >= 1)
);
CREATE UNIQUE INDEX UX_Lich_MotMui ON LichTiem (NguoiTiemId, VaccineId, MuiThu) WHERE TrangThai <> 'DaHuy';

/* QT4 + QT2: LichTiem - LoVaccine (n-n) qua MuiTiem; LichTiem - MuiTiem la 1-1 (UNIQUE) */
CREATE TABLE MuiTiem (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    LichTiemId    INT NOT NULL UNIQUE REFERENCES LichTiem(Id),
    LoVaccineId   INT NOT NULL REFERENCES LoVaccine(Id),
    NgayGioTiem   DATETIME2 NOT NULL,
    NhanVienId    INT NOT NULL REFERENCES NguoiDung(Id)
);
CREATE INDEX IX_MuiTiem_Lo ON MuiTiem (LoVaccineId);

CREATE TABLE NhacLich (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    LichTiemId   INT NOT NULL REFERENCES LichTiem(Id),
    ThoiDiemGui  DATETIME2 NOT NULL,
    Kenh         VARCHAR(20) NOT NULL REFERENCES DM_KenhNhac(Ma),
    TrangThai    VARCHAR(20) NOT NULL DEFAULT 'ChoGui' REFERENCES DM_TrangThaiNhac(Ma)
);

CREATE TABLE NhatKyHieuChinh (
    Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenBang     VARCHAR(50)   NOT NULL,
    MaBanGhi    VARCHAR(50)   NOT NULL,
    HanhDong    VARCHAR(20)   NOT NULL REFERENCES DM_HanhDong(Ma),
    GiaTriCu    NVARCHAR(MAX) NULL,
    GiaTriMoi   NVARCHAR(MAX) NULL,
    NguoiSuaId  INT           NULL REFERENCES NguoiDung(Id),
    ThoiDiem    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    LyDo        NVARCHAR(300) NULL
);
CREATE INDEX IX_NhatKy_BanGhi ON NhatKyHieuChinh (TenBang, MaBanGhi);
GO

CREATE TRIGGER TR_NhatKy_ChiThem ON NhatKyHieuChinh
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    RAISERROR (N'Nhat ky hieu chinh chi duoc them moi, khong duoc sua hoac xoa.', 16, 1);
END
GO

/* ===== Views truy vet (truy vet hai chieu) ===== */
CREATE VIEW vw_TruyVetTheoLo AS
SELECT lo.Id AS LoId, lo.SoLo, v.Ten AS Vaccine, sx.Ten AS NhaSanXuat, sx.QuocGia, lo.NgaySanXuat, lo.HanDung,
       nt.Id AS NguoiTiemId, nt.HoTen, nt.SoDienThoai,
       m.NgayGioTiem, nd.HoTen AS NhanVien
FROM MuiTiem m
JOIN LoVaccine lo ON lo.Id = m.LoVaccineId
JOIN Vaccine v    ON v.Id = lo.VaccineId
JOIN NhaSanXuat sx ON sx.Id = lo.NhaSanXuatId
JOIN LichTiem lt  ON lt.Id = m.LichTiemId
JOIN NguoiTiem nt ON nt.Id = lt.NguoiTiemId
JOIN NguoiDung nd ON nd.Id = m.NhanVienId;
GO
/* Doi chieu SoLuongCon voi so mui da tiem (phat hien lech) */
CREATE VIEW vw_DoiChieuTonLo AS
SELECT lo.Id AS LoId, lo.SoLo, lo.SoLuongNhap, lo.SoLuongCon,
       COUNT(m.Id) AS DaTiem,
       lo.SoLuongNhap - COUNT(m.Id) AS TonTinhLai,
       CASE WHEN lo.SoLuongCon = lo.SoLuongNhap - COUNT(m.Id) THEN 1 ELSE 0 END AS Khop
FROM LoVaccine lo LEFT JOIN MuiTiem m ON m.LoVaccineId = lo.Id
GROUP BY lo.Id, lo.SoLo, lo.SoLuongNhap, lo.SoLuongCon;
GO

/* ===== Du lieu mau ===== */
INSERT INTO Vaccine (Ten, NhaSanXuat, SoMui, KhoangCachNgay) VALUES
 (N'Vaccine cúm',        N'Sanofi', 1, NULL),
 (N'Vaccine viêm gan B', N'GSK',    3, 28),
 (N'Vaccine uốn ván',    N'Biological E', 2, 30);

INSERT INTO NhaSanXuat (Ten, QuocGia, DiaChi) VALUES
 (N'Sanofi Pasteur',                N'Pháp',    N'Lyon'),
 (N'GlaxoSmithKline Biologicals',   N'Bỉ',      N'Wavre'),
 (N'Biological E. Limited',         N'Ấn Độ',   N'Hyderabad'),
 (N'VABIOTECH',                     N'Việt Nam', N'Hà Nội');

INSERT INTO LoVaccine (VaccineId, NhaSanXuatId, SoLo, NgaySanXuat, HanDung, SoLuongNhap, SoLuongCon) VALUES
 (1, 1, 'VX-2409-A', '2026-09-12', '2027-10-12', 200, 200),
 (2, 2, 'VX-2410-B', '2026-10-01', '2027-06-30', 150, 150),
 (3, 3, 'VX-2411-C', '2026-09-15', '2027-03-15', 100, 100);

INSERT INTO NguoiTiem (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email) VALUES
 (N'Nguyễn Văn A', '1990-01-01', 'Nam', '0900000001', 'a@mail.com'),
 (N'Trần Thị B',   '1995-05-05', 'Nu',  '0900000002', 'b@mail.com'),
 (N'Lê Văn C',     '1985-08-20', 'Nam', '0900000003', 'c@mail.com');

INSERT INTO NguoiDung (SubjectOidc, HoTen, VaiTro, NguoiTiemId) VALUES
 ('oidc-admin-1', N'Quản trị 1',    'QuanTri',     NULL),
 ('oidc-nv-1',    N'BS. Lê C',      'NhanVienYTe', NULL),
 ('oidc-nd-1',    N'Nguyễn Văn A',  'NguoiDan',    1);

INSERT INTO LichTiem (NguoiTiemId, VaccineId, MuiThu, NgayHen, TrangThai) VALUES
 (1, 1, 1, '2026-10-05', 'ChoTiem'),
 (2, 2, 1, '2026-10-06', 'ChoTiem'),
 (3, 3, 1, '2026-10-07', 'ChoTiem');

INSERT INTO ChongChiDinh (NguoiTiemId, VaccineId, LyDo, TrangThai, NguoiXacNhanId, NgayXacNhan) VALUES
 (3, 3, N'Tiền sử dị ứng nặng với thành phần vaccine', 'DaXacNhan', 2, SYSUTCDATETIME());
GO

/* ===== Truy van thu =====
 SELECT * FROM vw_TruyVetTheoLo WHERE SoLo = 'VX-2409-A';
 SELECT * FROM vw_TruyVetTheoLo WHERE NguoiTiemId = 1;
 SELECT * FROM vw_DoiChieuTonLo;
 SELECT * FROM NhatKyHieuChinh WHERE TenBang = 'LoVaccine' AND MaBanGhi = '1' ORDER BY ThoiDiem DESC;
*/
GO

/* ################ PHAN 2: BIEN DONG LO ################ */
/* PATCH: them BIEN DONG LO vao CSDL TiemChungCongDong dang co (KHONG xoa du lieu).
   Chay 1 lan trong SSMS. Neu da chay script v2 ban moi (da co bang BienDongLo) thi KHONG can chay file nay. */
USE TiemChungCongDong;
GO

/* ===== Bien dong lo: nhap them / hong / hao hut / huy ===== */
CREATE TABLE DM_LoaiBienDong (Ma VARCHAR(20) PRIMARY KEY, Ten NVARCHAR(50) NOT NULL);
INSERT INTO DM_LoaiBienDong VALUES ('NhapThem',N'Nhập thêm'),('Hong',N'Hỏng'),('HaoHut',N'Hao hụt'),('Huy',N'Hủy');

CREATE TABLE BienDongLo (
    Id               BIGINT IDENTITY(1,1) PRIMARY KEY,
    LoVaccineId      INT           NOT NULL REFERENCES LoVaccine(Id),
    Loai             VARCHAR(20)   NOT NULL REFERENCES DM_LoaiBienDong(Ma),
    SoLuong          INT           NOT NULL,
    ThoiDiem         DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    NguoiThucHienId  INT           NOT NULL REFERENCES NguoiDung(Id),
    LyDo             NVARCHAR(300) NOT NULL,
    CONSTRAINT CK_BienDongLo_SoLuong CHECK (SoLuong > 0)
);
CREATE INDEX IX_BienDongLo_Lo ON BienDongLo (LoVaccineId, ThoiDiem);
GO

/* Nhat ky bien dong chi duoc them moi, khong sua/xoa */
CREATE TRIGGER TR_BienDongLo_ChiThem ON BienDongLo
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    RAISERROR (N'Bien dong lo chi duoc them moi, khong duoc sua hoac xoa.', 16, 1);
END
GO

DROP VIEW IF EXISTS vw_DoiChieuTonLo;
GO
/* Doi chieu SoLuongCon: Nhap(ban dau + nhap them) - da tiem - hong/hao hut/huy */
CREATE VIEW vw_DoiChieuTonLo AS
SELECT lo.Id AS LoId, lo.SoLo, lo.SoLuongNhap, lo.SoLuongCon,
       ISNULL(n.Them, 0)  AS NhapThem,
       ISNULL(m.DaTiem,0) AS DaTiem,
       ISNULL(g.Giam, 0)  AS HongHaoHutHuy,
       lo.SoLuongNhap - ISNULL(m.DaTiem,0) - ISNULL(g.Giam,0) AS TonTinhLai,
       CASE WHEN lo.SoLuongCon = lo.SoLuongNhap - ISNULL(m.DaTiem,0) - ISNULL(g.Giam,0)
            THEN 1 ELSE 0 END AS Khop
FROM LoVaccine lo
LEFT JOIN (SELECT LoVaccineId, COUNT(*)      AS DaTiem FROM MuiTiem GROUP BY LoVaccineId) m ON m.LoVaccineId = lo.Id
LEFT JOIN (SELECT LoVaccineId, SUM(SoLuong)  AS Them   FROM BienDongLo WHERE Loai = 'NhapThem' GROUP BY LoVaccineId) n ON n.LoVaccineId = lo.Id
LEFT JOIN (SELECT LoVaccineId, SUM(SoLuong)  AS Giam   FROM BienDongLo WHERE Loai <> 'NhapThem' GROUP BY LoVaccineId) g ON g.LoVaccineId = lo.Id;
GO

SELECT * FROM DM_LoaiBienDong;
SELECT * FROM vw_DoiChieuTonLo;

/* ################ PHAN 3: DANG NHAP ################ */
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
