using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Data;
using TokenAuthSystemMVC.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
    ?? throw new InvalidOperationException("Connection string not found.");

// Configure database.
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(connectionString));

// Add Identity API.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// Configure JWT tokens.
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("AUTHENTICATION_GOOGLE_CLIENT_ID") ??
            throw new InvalidOperationException("Google client id not found.");
        options.ClientSecret = Environment.GetEnvironmentVariable("AUTHENTICATION_GOOGLE_CLIENT_SECRET") ??
            throw new InvalidOperationException("Google client secret not found.");
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("AUTHENTICATION_MICROSOFT_CLIENT_ID") ??
            throw new InvalidOperationException("Microsoft client id not found.");
        options.ClientSecret = Environment.GetEnvironmentVariable("AUTHENTICATION_MICROSOFT_CLIENT_SECRET") ??
            throw new InvalidOperationException("Microsoft client secret not found.");
    })
    .AddGitHub(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("AUTHENTICATION_GITHUB_CLIENT_ID") ??
            throw new InvalidOperationException("Github client id not found.");
        options.ClientSecret = Environment.GetEnvironmentVariable("AUTHENTICATION_GITHUB_CLIENT_SECRET") ??
            throw new InvalidOperationException("Github client id not found.");
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration.GetSection("JWT:ValidIssuer").Value,
            ValidAudience = builder.Configuration.GetSection("JWT:ValidAudience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("JWT:SecretKey").Value ??
                    throw new InvalidOperationException("JWT secret key not found.")))
        };
    });

// Edit password requirments.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
});

// Dependency injection.
builder.Services.AddTransient<IJwtTokenProvider, JwtTokenProvider>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

// Enable sessions.
builder.Services.AddSession();

// Add support for Razor pages.
builder.Services.AddRazorPages();

// Enable MVC.
builder.Services.AddControllersWithViews();

// Create an object of application.
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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();

// Required for user data managing.
app.UseAuthentication();
// Required for login and registration.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
