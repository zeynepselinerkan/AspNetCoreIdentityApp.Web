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

builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(30)); // default 30 dk, otomatik logout i�in - hassas bilgiler de�i�ti�inde de�i�ir. Concurrency stamp ise identity ile ilgili de�il - e�zamanl�l�k i�in(benden �nce de�i�iklik olduysa uyar� g�nderme)-her g�ncellemede de�i�ir. Ayn� anda olursa bir tanesini kabul eder. Identity otomatik concurrency stamp kontrol ediyor.

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory())); // userpictures i�in

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
        policy.RequireClaim("City", "Kocaeli"); // Virg�lle izin verieln �ehirleri artt�rabilirim.
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
    opt.SlidingExpiration = true; // Her giri�te 60 g�n uzamas�n� sa�layacak. False olursa 61.g�n tekrar login sayfas�na y�nlendirir.
});

var app = builder.Build();

using (var scope=app.Services.CreateScope()) // 1 kere �al��acak.
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Uygulama e�er production environment ortam�nda aya�a kalkarsa hata sayfas�na gider ve hata sayfas�nda t�m exceptionlar� yakalay�p g�sterebiliriz.

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// �nce Authentication sonra Authorization yaz�l�r!!!
app.UseAuthentication(); // Kimlik Do�rulama --> �zellikle third party i�in gerekli.
app.UseAuthorization(); // Kimlik Yetkilendirme

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
