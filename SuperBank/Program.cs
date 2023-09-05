using SuperBank.Data;
using Microsoft.EntityFrameworkCore; // Import this namespace
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperBank.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using SuperBank.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

// Add services to the container.
builder.Services.AddRazorPages();

//builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlite("Data Source=superbank.db"));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite("Data Source=authorize.db"));

builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<BankAccountService>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;          // Minimum password length
    options.Password.RequireDigit = true;          // Require at least one digit
    options.Password.RequireNonAlphanumeric = true; // Require at least one special character
    options.Password.RequireUppercase = true;      // Require at least one uppercase letter
    options.Password.RequireLowercase = true;      // Require at least one lowercase letter
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();


app.UseAuthorization();


app.MapRazorPages();

app.Run();

