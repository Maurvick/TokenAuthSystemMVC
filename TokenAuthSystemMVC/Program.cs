using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TokenAuthSystemMvc.Server.Models;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Data;
using TokenAuthSystemMVC.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");

// Configure database.
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(connectionString));

// Add Identity API.
builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add support for Razor pages.
builder.Services.AddRazorPages();

builder.Services.AddAuthorization();

// Enable MVC.
builder.Services.AddControllersWithViews();

// Configure JWT tokens.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
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

// app.UseStatusCodePagesWithRedirects("/Identity/Account/AccessDenied");

// Create and assign roles. 
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = 
//        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    var roles = new[] { "Admin", "Manager", "Member" };

//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//            await roleManager.CreateAsync(new IdentityRole(role));
//    }
//}

// Seeding initial data. Create an admin.
//using (var scope = app.Services.CreateScope())
//{
//    var userManager =
//        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//    string adminEmail = "adminadmin@admin.com";
//    string adminPassword = "Test1234,";

//    if (await userManager.FindByEmailAsync(adminEmail) == null)
//    {
//        var user = new ApplicationUser();

//        user.UserName = adminEmail;
//        user.Email = adminEmail;
//        user.EmailConfirmed = true;

//        await userManager.CreateAsync(user, adminPassword);
//        await userManager.AddToRoleAsync(user, UserRoleModel.Admin);
//    }
//}

app.Run();
