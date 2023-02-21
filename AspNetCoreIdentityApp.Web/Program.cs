using AspNetCoreIdentityApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.OptionsModels;
using AspNetCoreIdentityApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityAppConnectionString"));
});

builder.Services.AddIdentityWithExtension();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings")); // app.dev jsondaki
builder.Services.AddScoped<IEmailService, EmailService>(); 

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";
    opt.LoginPath = new PathString("/Home/SignIn");
    opt.LogoutPath = new PathString("/Member/Logout");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    opt.SlidingExpiration = true; // Her giriþte 60 gün uzamasýný saðlayacak. False olursa 61.gün tekrar login sayfasýna yönlendirir.
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Önce Authentication sonra Authorization yazýlýr!!!
app.UseAuthentication(); // Kimlik Doðrulama --> Özellikle third party için gerekli.
app.UseAuthorization(); // Kimlik Yetkilendirme

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
