using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Core.OptionsModels;
using AspNetCoreIdentityApp.Service.Services;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentityApp.Web.ClaimProvider;
using Microsoft.AspNetCore.Authentication;
using AspNetCoreIdentityApp.Web.Requirements;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentityApp.Web.Seeds;
using AspNetCoreIdentityApp.Core.PermissionRoot;
using AspNetCoreIdentityApp.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityAppConnectionString"), options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
    });
});

builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(30)); // default 30 dk, otomatik logout için - hassas bilgiler deðiþtiðinde deðiþir. Concurrency stamp ise identity ile ilgili deðil - eþzamanlýlýk için(benden önce deðiþiklik olduysa uyarý gönderme)-her güncellemede deðiþir. Ayný anda olursa bir tanesini kabul eder. Identity otomatik concurrency stamp kontrol ediyor.

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory())); // userpictures için

builder.Services.AddIdentityWithExtension();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings")); // app.dev jsondaki
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpirationRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceVideoRequirementHandler>();
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("KocaeliPolicy", policy =>
    {
        policy.RequireClaim("City", "Kocaeli"); // Virgülle izin verieln þehirleri arttýrabilirim.
        //policy.RequireRole("Admin"); --> Birden fazla kural eklenebilir.
    });

    opt.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement()); 
    });

    opt.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceVideoRequirement() { ThresholdAge=18});
    });
    opt.AddPolicy("OrderPermissionPolicy1", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Read);
        policy.RequireClaim("Permission", Permission.Order.Delete);
        policy.RequireClaim("Permission", Permission.Stock.Delete);
    });
    opt.AddPolicy("OrderPermissionPolicy2", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Read);
     
    });
    opt.AddPolicy("OrderPermissionPolicy3", policy =>
    {
     
        policy.RequireClaim("Permission", Permission.Order.Delete);
 
    });
    opt.AddPolicy("OrderPermissionPolicy4", policy =>
    {
        policy.RequireClaim("Permission", Permission.Stock.Delete);
    });

});

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";
    opt.LoginPath = new PathString("/Home/SignIn");
    opt.LogoutPath = new PathString("/Member/Logout");
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    opt.SlidingExpiration = true; // Her giriþte 60 gün uzamasýný saðlayacak. False olursa 61.gün tekrar login sayfasýna yönlendirir.
});

var app = builder.Build();

using (var scope=app.Services.CreateScope()) // 1 kere çalýþacak.
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Uygulama eðer production environment ortamýnda ayaða kalkarsa hata sayfasýna gider ve hata sayfasýnda tüm exceptionlarý yakalayýp gösterebiliriz.

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
