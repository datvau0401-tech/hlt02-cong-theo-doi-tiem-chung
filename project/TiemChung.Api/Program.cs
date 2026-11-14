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

// Đăng ký "ai làm việc gì": khi cần IVaccineRepository thì dùng VaccineRepository, v.v.
builder.Services.AddScoped<IVaccineRepository, VaccineRepository>();
builder.Services.AddScoped<ILoVaccineRepository, LoVaccineRepository>();
builder.Services.AddScoped<LoVaccineService>();
builder.Services.AddScoped<IMuiTiemRepository, MuiTiemRepository>();
builder.Services.AddScoped<MuiTiemService>();
builder.Services.AddScoped<IBienDongLoRepository, BienDongLoRepository>();
builder.Services.AddScoped<BienDongLoService>();

var app = builder.Build();

app.MapControllers();
app.Run();