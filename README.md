# Cổng theo dõi tiêm chủng cộng đồng (HLT-02)

Đồ án cuối kỳ môn Phương pháp phát triển phần mềm hướng đối tượng.

- Sinh viên: Nguyễn Tấn Đạt, MSSV 1250080032, lớp 12_ĐH_CNPM1
- Giảng viên hướng dẫn: Phạm Trọng Huynh
- Trường Đại học Tài nguyên và Môi trường TP. Hồ Chí Minh, Khoa Công nghệ thông tin

## Giới thiệu
Hệ thống quản lý người tiêm, vaccine và lô vaccine (kèm nhà sản xuất), chống chỉ định, lịch tiêm, ghi nhận mũi tiêm, truy vết theo lô và lịch sử hiệu chỉnh. Có 3 vai trò: Quản trị, Nhân viên y tế, Người dân (tự đăng ký tài khoản).

## Công nghệ
- Giao diện: React + Vite
- API: ASP.NET Core, kiến trúc Clean Architecture (Domain, Application, Infrastructure, Api)
- CSDL: SQL Server (Entity Framework Core)
- Xác thực: cookie, mật khẩu băm PBKDF2

## Cấu trúc thư mục
| Thư mục | Nội dung |
|---|---|
| `project/` | Mã nguồn API (.NET) và giao diện (React) |
| `sql/` | Script tạo CSDL, nạp dữ liệu demo, truy vấn demo |
| `Sodo/` | Sơ đồ: file `.drawio` và ảnh `.png` theo số hình trong báo cáo |
| `AnhSanPham/` | Ảnh chụp màn hình sản phẩm (Chương VI) |
| `HTL_02_1250080032.docx` | Báo cáo đồ án (bản Word) |

## Cài đặt và chạy
Yêu cầu: .NET SDK, Node.js, SQL Server (hoặc SQL Server Express), SSMS.

1. **Tạo CSDL:** mở SSMS, chạy `sql/TaoCSDL_DayDu.sql`.
2. **Nạp dữ liệu demo (chạy 1 lần):** chọn CSDL `TiemChungCongDong`, chạy `sql/Seed_DemoData.sql`.
3. **Cấu hình chuỗi kết nối:** sửa `ConnectionStrings` trong `appsettings.json` của dự án Api cho đúng máy của bạn (không đưa mật khẩu thật lên GitHub).
4. **Chạy API:** vào thư mục dự án Api, chạy `dotnet run` (mặc định cổng 5080).
5. **Chạy giao diện:** vào thư mục React, chạy `npm install` rồi `npm run dev`, mở địa chỉ Vite hiển thị (giao diện gọi `/api` qua proxy tới cổng 5080).

## Tài khoản demo
| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Người dân | `hoa` | `Demo@1234` |
| Người dân | `nguyenvana` | `Demo@1234` |

Tài khoản quản trị và nhân viên y tế: xem phần nạp dữ liệu trong `sql/`.

## Truy vấn demo
`sql/Demo_SQL.sql` gồm các khối truy vấn đánh số (tồn kho theo lô, truy vết theo lô và theo người, đối chiếu tồn kho, lịch quá hạn, chống chỉ định, nơi sản xuất, lịch sử hiệu chỉnh) và các khối chứng minh ràng buộc toàn vẹn (tự ROLLBACK).
Để luyện demo nhiều lần: chạy `Demo_1_SaoLuuMoc.sql` một lần, sau mỗi buổi luyện chạy `Demo_2_KhoiPhucMoc.sql`.
