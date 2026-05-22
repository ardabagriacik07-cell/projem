using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")) &&
    string.IsNullOrWhiteSpace(builder.Configuration["urls"]))
{
    builder.WebHost.UseUrls("http://localhost:5106");
}

builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterClient", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true);
    });
});
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=servis.db";
var sqliteConnectionBuilder = new SqliteConnectionStringBuilder(rawConnectionString);

if (string.IsNullOrWhiteSpace(sqliteConnectionBuilder.DataSource) == false &&
    Path.IsPathRooted(sqliteConnectionBuilder.DataSource) == false &&
    sqliteConnectionBuilder.DataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase) == false &&
    sqliteConnectionBuilder.Mode != SqliteOpenMode.Memory)
{
    sqliteConnectionBuilder.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqliteConnectionBuilder.DataSource);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseSqlite(sqliteConnectionBuilder.ToString())
        .ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles(); // BUNU EKLEDİK
app.UseRouting();
app.UseCors("FlutterClient");

app.UseSession();
app.UseAuthorization();

// app.MapStaticAssets(); BUNU SİLDİK

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Uye}/{action=Giris}/{id?}");

app.MapFallbackToFile("/mobile/{*path:nonfile}", "mobile/index.html");

// .WithStaticAssets(); BUNU SİLDİK

app.Run();
