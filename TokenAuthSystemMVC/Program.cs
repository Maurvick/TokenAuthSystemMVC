using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Data;
using TokenAuthSystemMVC.Interfaces;
using TokenAuthSystemMVC.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("AuthDbContextConnection") ?? 
    throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

// Configure database.
builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
    options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Enable MVC.
builder.Services.AddControllersWithViews();

// Dependency injection of TokenService class.
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Add support for Razor pages.
builder.Services.AddRazorPages();

// Enable sessions.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

// Configure JWT tokens.
builder.Services.AddAuthentication(options =>
{
    // Set options as IdentityConstats, so identity razor pages would work properly.
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["JWT:ValidIssuer"],
        ValidAudience = configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"] ??
            throw new InvalidOperationException("JWT secret string 'JWT:SecretKey' not found.")))
    };
});

// Edit password requirments.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
});

var app = builder.Build();

// Add token to headers if exists.
app.UseSession();

app.Use(async (context, next) =>
{
    var token = context.Session.GetString("Token");

    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Append("Authorization", "Bearer " + token);
    }

    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
