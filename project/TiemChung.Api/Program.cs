using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TiemChung.Application.Abstractions;
using TiemChung.Application.Services;
using TiemChung.Infrastructure;
using TiemChung.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Cho API biết cách kết nối SQL Server
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ===== Đăng nhập bằng cookie =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "tiemchung.auth";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
        // API trả mã lỗi thay vì chuyển hướng sang trang đăng nhập (trang web tự xử lý 401)
        o.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
        o.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
    });

// Mặc định mọi API chỉ dành cho nhân viên y tế / quản trị viên.
// Chỗ nào cho người dân hoặc ẩn danh dùng thì ghi [Authorize(Roles = "NguoiDan")] / [Authorize] / [AllowAnonymous]
builder.Services.AddAuthorization(o =>
    o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireRole("NhanVienYTe", "QuanTri").Build());

// Đăng ký "ai làm việc gì": khi cần IVaccineRepository thì dùng VaccineRepository, v.v.
builder.Services.AddScoped<IVaccineRepository, VaccineRepository>();
builder.Services.AddScoped<VaccineService>();
builder.Services.AddScoped<ILoVaccineRepository, LoVaccineRepository>();
builder.Services.AddScoped<LoVaccineService>();
builder.Services.AddScoped<IMuiTiemRepository, MuiTiemRepository>();
builder.Services.AddScoped<MuiTiemService>();
builder.Services.AddScoped<IBienDongLoRepository, BienDongLoRepository>();
builder.Services.AddScoped<BienDongLoService>();
builder.Services.AddScoped<INguoiTiemRepository, NguoiTiemRepository>();
builder.Services.AddScoped<NguoiTiemService>();
builder.Services.AddScoped<ILichTiemRepository, LichTiemRepository>();
builder.Services.AddScoped<LichTiemService>();
builder.Services.AddScoped<IChongChiDinhRepository, ChongChiDinhRepository>();
builder.Services.AddScoped<ChongChiDinhService>();
builder.Services.AddScoped<INhatKyRepository, NhatKyRepository>();
builder.Services.AddScoped<NhatKyService>();
builder.Services.AddScoped<ITruyVetRepository, TruyVetRepository>();
builder.Services.AddScoped<TruyVetService>();
builder.Services.AddScoped<IThongKeRepository, ThongKeRepository>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaiKhoanService>();
builder.Services.AddScoped<CaNhanService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
