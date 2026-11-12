/* ============================================================
   DEMO SQL - TiemChungCongDong
   Cach dung: chon CSDL TiemChungCongDong, boi den TUNG KHOI (tu dau "-- [n]" den GO) roi bam F5.
   Cac khoi 11-16 la demo rang buoc: tu ROLLBACK nen KHONG lam hong du lieu.
   ============================================================ */
USE TiemChungCongDong;
GO

/* ---------- PHAN A. TRUY VAN NGHIEP VU ---------- */

-- [1] Ton kho theo lo + canh bao han dung (nguong lay tu bang tham so he thong)
SELECT lo.SoLo, v.Ten AS Vaccine, lo.HanDung, lo.SoLuongNhap, lo.SoLuongCon,
       DATEDIFF(DAY, CAST(GETDATE() AS DATE), lo.HanDung) AS NgayConLai,
       CASE WHEN lo.HanDung < CAST(GETDATE() AS DATE) THEN N'Đã hết hạn'
            WHEN lo.HanDung <= DATEADD(DAY,
                 (SELECT TRY_CAST(GiaTri AS INT) FROM ThamSoHeThong WHERE TenThamSo = 'SoNgayCanhBaoHetHan'),
                 CAST(GETDATE() AS DATE)) THEN N'Sắp hết hạn'
            WHEN lo.SoLuongCon = 0 THEN N'Hết hàng'
            ELSE N'Còn hạn' END AS TinhTrang
FROM LoVaccine lo JOIN Vaccine v ON v.Id = lo.VaccineId
ORDER BY lo.HanDung;
GO

-- [2] TRUY VET THEO LO: lo nay da tiem cho nhung ai? (doi 'VX-2409-A' bang so lo can tra)
SELECT * FROM vw_TruyVetTheoLo WHERE SoLo = 'JE-01' ORDER BY NgayGioTiem;
GO

-- [3] TRUY VET THEO NGUOI: nguoi nay da tiem bang lo nao?
SELECT * FROM vw_TruyVetTheoLo WHERE HoTen LIKE N'%Tấn Trường%' ORDER BY NgayGioTiem;
GO
-- [4] Doi chieu ton kho: cot Khop phai = 1 het (Nhap - da tiem - hong/hao hut/huy = SoLuongCon)
SELECT * FROM vw_DoiChieuTonLo;
GO



-- [5] Lich da qua han ma chua tiem (can goi nhac)
SELECT nt.HoTen, nt.SoDienThoai, v.Ten AS Vaccine, lt.MuiThu, lt.NgayHen,
       DATEDIFF(DAY, lt.NgayHen, CAST(GETDATE() AS DATE)) AS TreSoNgay
FROM LichTiem lt
JOIN NguoiTiem nt ON nt.Id = lt.NguoiTiemId
JOIN Vaccine v ON v.Id = lt.VaccineId
WHERE lt.TrangThai = 'ChoTiem' AND lt.NgayHen < CAST(GETDATE() AS DATE)
ORDER BY lt.NgayHen;
GO

-- [6] Nguoi co chong chi dinh da xac nhan (he thong se chan dat lich/tiem)
SELECT nt.HoTen, v.Ten AS Vaccine, c.LyDo, c.TrangThai, nd.HoTen AS NguoiXacNhan, c.NgayXacNhan
FROM ChongChiDinh c
JOIN NguoiTiem nt ON nt.Id = c.NguoiTiemId
JOIN Vaccine v ON v.Id = c.VaccineId
LEFT JOIN NguoiDung nd ON nd.Id = c.NguoiXacNhanId
WHERE c.TrangThai = 'DaXacNhan';
GO

-- [7] Thong ke so mui da tiem theo vaccine va theo lo
SELECT v.Ten AS Vaccine, lo.SoLo, COUNT(m.Id) AS SoMuiDaTiem
FROM Vaccine v
JOIN LoVaccine lo ON lo.VaccineId = v.Id
LEFT JOIN MuiTiem m ON m.LoVaccineId = lo.Id
GROUP BY v.Ten, lo.SoLo
ORDER BY v.Ten, lo.SoLo;
GO

-- [8] Nguoi da tiem du so mui cua mot vaccine (HAVING)
SELECT nt.HoTen, v.Ten AS Vaccine, v.SoMui, COUNT(m.Id) AS DaTiem
FROM LichTiem lt
JOIN NguoiTiem nt ON nt.Id = lt.NguoiTiemId
JOIN Vaccine v ON v.Id = lt.VaccineId
JOIN MuiTiem m ON m.LichTiemId = lt.Id
GROUP BY nt.Id, nt.HoTen, v.Id, v.Ten, v.SoMui
HAVING COUNT(m.Id) >= v.SoMui;
GO

-- [9] Bien dong cua tung lo (nhap them / hong / hao hut / huy)
SELECT lo.SoLo, b.ThoiDiem, d.Ten AS Loai, b.SoLuong, nd.HoTen AS NguoiThucHien, b.LyDo
FROM BienDongLo b
JOIN LoVaccine lo ON lo.Id = b.LoVaccineId
JOIN DM_LoaiBienDong d ON d.Ma = b.Loai
JOIN NguoiDung nd ON nd.Id = b.NguoiThucHienId
ORDER BY b.ThoiDiem DESC;
GO

-- [10] LICH SU HIEU CHINH: ai sua gi, luc nao (gia tri cu -> moi lay tu JSON)
SELECT n.ThoiDiem, ISNULL(nd.HoTen, N'(không rõ)') AS NguoiSua, n.TenBang, n.MaBanGhi, n.HanhDong,
       JSON_VALUE(n.GiaTriCu,  '$.ngayHen') AS NgayHenCu,
       JSON_VALUE(n.GiaTriMoi, '$.ngayHen') AS NgayHenMoi,
       n.LyDo
FROM NhatKyHieuChinh n
LEFT JOIN NguoiDung nd ON nd.Id = n.NguoiSuaId
WHERE n.TenBang = 'LichTiem'
ORDER BY n.ThoiDiem DESC;
GO

-- [9b] NOI SAN XUAT: moi lo do ai san xuat, o dau, san xuat ngay nao
SELECT lo.SoLo, v.Ten AS Vaccine, sx.Ten AS NhaSanXuat, sx.QuocGia, lo.NgaySanXuat, lo.HanDung, lo.SoLuongCon
FROM LoVaccine lo
JOIN Vaccine v ON v.Id = lo.VaccineId
JOIN NhaSanXuat sx ON sx.Id = lo.NhaSanXuatId
ORDER BY sx.QuocGia, sx.Ten, lo.SoLo;
GO

-- [9c] THU HOI THEO NHA SAN XUAT: neu mot nha san xuat bi thu hoi thi nhung ai da tiem lo cua ho? (doi ten nha san xuat)
SELECT sx.Ten AS NhaSanXuat, t.SoLo, t.HoTen, t.SoDienThoai, t.NgayGioTiem
FROM vw_TruyVetTheoLo t
JOIN LoVaccine lo ON lo.Id = t.LoId
JOIN NhaSanXuat sx ON sx.Id = lo.NhaSanXuatId
WHERE sx.Ten = N'Sanofi Pasteur'
ORDER BY t.SoLo, t.NgayGioTiem;
GO

-- [9d] Thong ke theo noi san xuat: so lo va so mui da tiem
SELECT sx.Ten AS NhaSanXuat, sx.QuocGia, COUNT(DISTINCT lo.Id) AS SoLo,
       COUNT(m.Id) AS SoMuiDaTiem
FROM NhaSanXuat sx
LEFT JOIN LoVaccine lo ON lo.NhaSanXuatId = sx.Id
LEFT JOIN MuiTiem m ON m.LoVaccineId = lo.Id
GROUP BY sx.Id, sx.Ten, sx.QuocGia
ORDER BY SoMuiDaTiem DESC;
GO

/* ---------- PHAN B. DEMO RANG BUOC TOAN VEN (tu rollback) ---------- */

-- [11] Ton con khong duoc lon hon so nhap (CHECK CK_Lo_SoLuongCon)
BEGIN TRAN;
BEGIN TRY
    INSERT INTO LoVaccine (VaccineId, SoLo, HanDung, SoLuongNhap, SoLuongCon)
    VALUES (1, 'TEST-01', '2027-01-01', 10, 20);
    PRINT N'SAI: lẽ ra phải bị chặn';
END TRY
BEGIN CATCH
    PRINT N'Bị chặn đúng: ' + ERROR_MESSAGE();
END CATCH;
ROLLBACK TRAN;
GO

-- [12] Ngay sinh khong duoc o tuong lai (CHECK CK_NguoiTiem_NgaySinh)
BEGIN TRAN;
BEGIN TRY
    INSERT INTO NguoiTiem (HoTen, NgaySinh) VALUES (N'Test', '2999-01-01');
    PRINT N'SAI: lẽ ra phải bị chặn';
END TRY
BEGIN CATCH
    PRINT N'Bị chặn đúng: ' + ERROR_MESSAGE();
END CATCH;
ROLLBACK TRAN;
GO

-- [13] Khong dat trung lich con hieu luc cung nguoi - vaccine - mui (unique index co dieu kien)
BEGIN TRAN;
BEGIN TRY
    INSERT INTO LichTiem (NguoiTiemId, VaccineId, MuiThu, NgayHen, TrangThai)
    SELECT TOP 1 NguoiTiemId, VaccineId, MuiThu, NgayHen, 'ChoTiem' FROM LichTiem WHERE TrangThai <> 'DaHuy';
    PRINT N'SAI: lẽ ra phải bị chặn';
END TRY
BEGIN CATCH
    PRINT N'Bị chặn đúng: ' + ERROR_MESSAGE();
END CATCH;
ROLLBACK TRAN;
GO

-- [14] Mot lich chi duoc co toi da 1 mui tiem (UNIQUE LichTiemId). Can co it nhat 1 dong trong MuiTiem
BEGIN TRAN;
BEGIN TRY
    INSERT INTO MuiTiem (LichTiemId, LoVaccineId, NgayGioTiem, NhanVienId)
    SELECT TOP 1 LichTiemId, LoVaccineId, SYSUTCDATETIME(), NhanVienId FROM MuiTiem;
    PRINT N'SAI: lẽ ra phải bị chặn';
END TRY
BEGIN CATCH
    PRINT N'Bị chặn đúng: ' + ERROR_MESSAGE();
END CATCH;
ROLLBACK TRAN;
GO

-- [15] Chong chi dinh da xac nhan phai co nguoi xac nhan (CHECK CK_CCD_XacNhan)
BEGIN TRAN;
BEGIN TRY
    INSERT INTO ChongChiDinh (NguoiTiemId, VaccineId, LyDo, TrangThai)
    VALUES (1, 1, N'Test', 'DaXacNhan');
    PRINT N'SAI: lẽ ra phải bị chặn';
END TRY
BEGIN CATCH
    PRINT N'Bị chặn đúng: ' + ERROR_MESSAGE();
END CATCH;
ROLLBACK TRAN;
GO

-- [16] Khong xoa duoc vaccine da co lo (khoa ngoai) va khong sua duoc nhat ky (trigger INSTEAD OF)
BEGIN TRAN;
BEGIN TRY
    DELETE FROM Vaccine WHERE Id = 1;
    PRINT N'SAI: lẽ ra phải bị chặn';
END TRY
BEGIN CATCH
    PRINT N'Bị chặn đúng (khóa ngoại): ' + ERROR_MESSAGE();
END CATCH;
ROLLBACK TRAN;
GO

BEGIN TRAN;
BEGIN TRY
    INSERT INTO NhatKyHieuChinh (TenBang, MaBanGhi, HanhDong) VALUES ('Demo', '0', 'THEM');
    UPDATE NhatKyHieuChinh SET LyDo = N'sửa trộm' WHERE TenBang = 'Demo';
    PRINT N'SAI: lẽ ra phải bị chặn';
END TRY
BEGIN CATCH
    PRINT N'Bị chặn đúng (trigger): ' + ERROR_MESSAGE();
END CATCH;
ROLLBACK TRAN;
GO
SELECT * FROM Vaccine ORDER By Id DESC; 
SELECT * FROM NguoiDung ORDER By Id DESC; 
SELECT * FROM LichTiem  ORDER By Id DESC;
