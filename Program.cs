using BlojayElectronics.Data;
using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration (environment variables will override)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddRazorPages();

// --- Determine the connection string ---
string connectionString;

if (builder.Environment.IsProduction())
{
    // Use SQLite with the production path (Render's /app/data)
    connectionString = "Data Source=/app/data/BlojayElectronics.db";
    Console.WriteLine("🟢 Production environment detected. Using SQLite with /app/data path.");
}
else
{
    // For development, use the connection string from config or default
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=BlojayElectronics.db";
}

Console.WriteLine($"📌 Connection string: {connectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    // Seed admin if none exists
    if (!dbContext.Admins.Any())
    {
        var admin = new Admin
        {
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            FullName = "System Administrator",
            CreatedAt = DateTime.UtcNow
        };
        dbContext.Admins.Add(admin);
        dbContext.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();

app.Run();

static string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}