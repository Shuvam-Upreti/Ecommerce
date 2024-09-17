using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Mover.Core.Entities;
using Mover.Core.Entities.UserManagement;
using Mover.Core.Repository.Interfaces;
using Mover.HttpUtility;
using Mover.Infrastructure.Context;
using Mover.Infrastructure.Repository.Implementation;
using Mover.Logging;
using Mover.Middleware;
using Mover.SeedData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityUserDbContext>(options =>
{
    options.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("MoverConnection"), b => b.MigrationsAssembly("Mover.Infrastructure"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<IdentityUserDbContext>();
builder.Services.AddDbContext<MoverContext>(options =>
{
    options.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("MoverConnection"), b => b.MigrationsAssembly("Mover.Infrastructure"));
    // options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
});

RegisterServices(builder.Services);

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#";
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/home/error";
    options.SlidingExpiration = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
});

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/jpeg", "image/png", "image/jpg" });
});
builder.Services.AddSession();
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.SeedData();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Default HSTS with no parameters
}

AppHttpContext.Services = app.Services.CreateScope().ServiceProvider;

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

// Security headers
app.UseHsts();  // This version works without custom parameters in .NET 8
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    await next();
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void RegisterServices(IServiceCollection services)
{
    services.Scan(scan => scan
        .FromAssembliesOf(typeof(IBaseRepository<>), typeof(BaseRepository<>), typeof(ISeriLogger))
        .AddClasses()
        .AsSelf()
        .AsImplementedInterfaces()
        .WithScopedLifetime());
}
