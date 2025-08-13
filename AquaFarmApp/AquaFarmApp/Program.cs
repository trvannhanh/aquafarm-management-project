using AquaFarmApp.Data;
using AquaFarmApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

// Thêm DbContext
builder.Services.AddDbContext<AquaFarmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<User, IdentityRole<int>>( options =>
{
    options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt trong mật khẩu
    options.Password.RequiredLength = 8; // Đặt độ dài tối thiểu của mật khẩu
    options.Password.RequireUppercase = false; // Yêu cầu ít nhất một chữ hoa
    options.Password.RequireLowercase = false; // Yêu cầu ít nhất một chữ thường
    options.User.RequireUniqueEmail = true; // Yêu cầu email duy nhất
    options.SignIn.RequireConfirmedAccount = false; // Không yêu cầu xác nhận tài khoản
    options.SignIn.RequireConfirmedEmail = false; // Không yêu cầu xác nhận email
    options.SignIn.RequireConfirmedPhoneNumber = false; // Không yêu cầu xác nhận số điện thoại
})
    .AddEntityFrameworkStores<AquaFarmContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AquaFarm API", Version = "v1" });
});
var app = builder.Build();


if(!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}")
//     .WithStaticAssets();
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
