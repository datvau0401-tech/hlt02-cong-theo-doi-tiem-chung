# Hướng dẫn cập nhật dự án Tiêm chủng cộng đồng

File `TiemChung_CapNhat.zip` gồm 21 file đã sửa hoặc thêm mới, giữ nguyên cấu trúc thư mục của dự án.

## Cần có

- SQL Server (CSDL `TiemChungCongDong` đã tạo bằng script v2 và đã chạy patch biến động lô)
- .NET 8 SDK (hoặc Visual Studio)
- Node.js (để chạy giao diện)

## Bước 1. Sao lưu

Copy cả thư mục `TiemChung` ra chỗ khác trước khi đè file, để quay lại được nếu có lỗi.

## Bước 2. Giải nén đè

Giải nén `TiemChung_CapNhat.zip` vào thư mục `TiemChung` (thư mục chứa `TiemChung.slnx`). Chọn **Replace** khi Windows hỏi ghi đè.

Sau khi đè, các file sau phải nằm đúng chỗ:

```
TiemChung/
├─ TiemChung.Api/
│  ├─ Program.cs                          (sửa)
│  └─ Controllers/
│     ├─ VaccineController.cs             (sửa)
│     └─ ChongChiDinhController.cs        (mới)
├─ TiemChung.Application/
│  ├─ Abstractions/
│  │  ├─ IRepositories.cs                 (sửa)
│  │  └─ IChongChiDinhRepository.cs       (mới)
│  └─ Services/
│     ├─ VaccineService.cs                (mới)
│     └─ ChongChiDinhService.cs           (mới)
├─ TiemChung.Domain/Entities/
│  ├─ NguoiTiem.cs, NguoiDung.cs, LichTiem.cs, Vaccine.cs, ChongChiDinh.cs   (sửa)
├─ TiemChung.Infrastructure/
│  ├─ AppDbContext.cs                     (sửa)
│  └─ Repositories/
│     ├─ VaccineRepository.cs             (sửa)
│     └─ ChongChiDinhRepository.cs        (mới)
└─ tiemchung-web/src/
   ├─ main.jsx, App.jsx, App.css, NhatKyPage.jsx   (sửa)
   └─ VaccinePage.jsx, ChongChiDinhPage.jsx        (mới)
```

## Bước 3. Cập nhật CSDL (chỉ làm nếu chưa làm)

Mở SSMS, chạy file `Patch_DangNhap.sql` một lần. File này thêm cột tên đăng nhập và mật khẩu cho bảng `NguoiDung`. Chạy lại nhiều lần vẫn an toàn.

Tài khoản demo sau khi chạy:

| Tên đăng nhập | Mật khẩu   | Vai trò        | Quyền thêm/sửa vaccine |
|---------------|------------|----------------|------------------------|
| `admin`       | `Admin@123` | Quản trị viên  | Có                     |
| `bsc`         | `Bsc@123`   | Nhân viên y tế | Không (chỉ xem)        |

## Bước 4. Chạy API

Kiểm tra chuỗi kết nối trong `TiemChung.Api/appsettings.json` (tên server, tài khoản, mật khẩu SQL), rồi chạy một trong hai cách:

- Visual Studio: mở `TiemChung.slnx`, đặt `TiemChung.Api` làm Startup Project, nhấn F5.
- Dòng lệnh:
  ```
  cd TiemChung.Api
  dotnet run
  ```

API phải chạy ở `http://localhost:5080`, vì `vite.config.js` đang trỏ `/api` về cổng này. Nếu API chạy ở cổng khác, sửa số cổng trong `tiemchung-web/vite.config.js` (dòng `proxy`) hoặc trong `Properties/launchSettings.json`.

Nếu báo lỗi khi build, gửi nguyên nội dung lỗi cho mình để sửa.

## Bước 5. Chạy giao diện

Mở cửa sổ lệnh khác:

```
cd tiemchung-web
npm install
npm run dev
```

Mở địa chỉ hiện ra (thường là `http://localhost:5173`), đăng nhập bằng `admin` / `Admin@123`.

## Bước 6. Thử nhanh các chức năng

1. **Vaccine:** đăng nhập `admin`, thêm một vaccine mới (ví dụ 2 mũi, cách nhau 28 ngày). Đăng nhập `bsc` thì thấy danh sách nhưng không có form thêm.
2. **Người tiêm:** thêm một người, sửa lại tên, rồi mở **Lịch sử hiệu chỉnh** để xem giá trị cũ → mới.
3. **Chống chỉ định:** ghi nhận cho một người và một vaccine, bấm **Xác nhận**. Sau đó vào **Lịch tiêm**, đặt lịch cho đúng cặp đó thì hệ thống phải từ chối.
4. **Lịch tiêm → Ghi nhận mũi tiêm:** đặt lịch, ghi nhận mũi tiêm, kiểm tra số lượng còn của lô giảm 1.
5. **Truy vết lô:** chọn lô vừa tiêm, phải thấy tên người đã tiêm.
6. **Đăng xuất:** bấm nút ở cuối thanh bên, phải quay về trang đăng nhập.

## Lỗi thường gặp

| Hiện tượng | Cách xử lý |
|---|---|
| "Không kết nối được API" | API chưa chạy, hoặc sai cổng trong `vite.config.js` |
| "Sai tên đăng nhập hoặc mật khẩu" | Chưa chạy `Patch_DangNhap.sql`, hoặc gõ sai mật khẩu (có phân biệt hoa/thường) |
| Trang quay về đăng nhập liên tục | Cookie hết hạn (8 giờ) hoặc API khởi động lại; đăng nhập lại |
| Lỗi `Invalid column name 'TenDangNhap'` | Chưa chạy `Patch_DangNhap.sql` trên đúng CSDL |
| Lỗi `Cannot insert NULL into SubjectOidc` | Chỉ xảy ra nếu thêm `NguoiDung` bằng code; bản này chưa có chức năng đó |
| Lỗi khi lưu nhật ký / bảng có trigger | Báo lại nguyên văn lỗi để mình chỉnh `AppDbContext` |

## Lưu ý bảo mật

- `appsettings.json` đang chứa mật khẩu SQL (`sa` / `123`). Đổi mật khẩu và không gửi file này cho người khác.
- Mật khẩu tài khoản demo là mật khẩu mẫu. Hãy đổi trước khi dùng thật.

## Ba vai trò và đăng ký tài khoản

| Vai trò | Ai tạo tài khoản | Được làm gì |
|---|---|---|
| Quản trị viên (`QuanTri`) | Có sẵn `admin`; admin khác do admin cấp | Mọi chức năng của bác sĩ, thêm/sửa danh mục vaccine, trang **Tài khoản** (cấp tài khoản bác sĩ/quản trị, đặt lại mật khẩu) |
| Bác sĩ / nhân viên y tế (`NhanVienYTe`) | Quản trị viên cấp ở trang **Tài khoản** | Quản lý lô, biến động, người tiêm, chống chỉ định, lịch, ghi nhận mũi tiêm, truy vết, xem nhật ký. Chỉ xem danh mục vaccine |
| Người dân (`NguoiDan`) | **Tự đăng ký** ở trang đăng nhập (nút "Đăng ký") | Chỉ thấy trang **Trang của tôi**: xem hồ sơ, đặt và hủy lịch của mình, xem lịch sử tiêm và chống chỉ định của mình |

- Đăng ký xong là đăng nhập luôn. Mật khẩu từ 8 ký tự, có cả chữ và số.
- Người dân không tự chọn được vai trò. Server luôn tạo tài khoản với vai trò người dân, và người dân chỉ truy cập được dữ liệu của chính mình.
- Tài khoản người dân có sẵn trong CSDL mẫu (`Nguyễn Văn A`) chưa có mật khẩu. Quản trị viên vào **Tài khoản** → **Cấp đăng nhập** để đặt tên đăng nhập và mật khẩu cho tài khoản đó.
- Người dân tự đăng ký sẽ tạo hồ sơ người tiêm mới. Hồ sơ do nhân viên đã nhập trước đó không tự gộp vào.

## Chưa làm

- Form cho bảng `NhacLich` (nhắc lịch tiêm).
- Khóa/mở khóa tài khoản, quên mật khẩu qua email.
- Giới hạn số lần đăng nhập sai và chống đăng ký hàng loạt.
