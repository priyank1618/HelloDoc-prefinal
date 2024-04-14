using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.ViewModel;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set your desired timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddScoped<IPatient_Request, Patient_Requestrepo>();
builder.Services.AddScoped<IFamily_Request,Family_Requestrepo>();
builder.Services.AddScoped<IConcierge_Request,Concierge_Requestrepo>();
builder.Services.AddScoped<IBusiness_Request,Business_Requestrepo>();
builder.Services.AddScoped<IAddFile,AddFilerepo>();
builder.Services.AddScoped<IHome,HomeRepo>();
builder.Services.AddScoped<IAdminDashBoard,AdminDashBoardrepo>();
builder.Services.AddScoped<IProviderDashBoard, ProviderDashBoard>();
builder.Services.AddScoped<IAdminAction,AdminAction>();
builder.Services.AddScoped<IAccountsAccess,AccountsAccess>();
builder.Services.AddScoped<IUploadProvider,UploadProvider>();
builder.Services.AddScoped<IEmailService, EmailServicerepo>();
builder.Services.AddScoped<IAdminDashboardRecords,AdminDashboardRecords>();
builder.Services.AddScoped<IPasswordHasher<Patient>,PasswordHasher<Patient>>();
builder.Services.AddScoped<IPasswordHasher<Patient_login>,PasswordHasher<Patient_login>>();
builder.Services.AddScoped<IPasswordHasher<AdminProfile>,PasswordHasher<AdminProfile>>();
builder.Services.AddScoped<IPasswordHasher<CreateAccount>,PasswordHasher<CreateAccount>>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddNotyf(config => {
    config.DurationInSeconds = 5; config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
    config.IsDismissable = false;
});

//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });
//Jwt configuration ends here 


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
app.UseRotativa();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseNotyf();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
