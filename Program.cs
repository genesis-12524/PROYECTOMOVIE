using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using CloudinaryDotNet;
using PROYECTOMOVIE.interfaze;
using PROYECTOMOVIE.Models.config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de Identity con tu modelo Usuario - MODIFICADO
builder.Services.AddIdentity<Usuario, IdentityRole>(options => 
{
    // Cambiar a false para desarrollo (no requiere confirmación de email)
    options.SignIn.RequireConfirmedAccount = false;
    
    // Configuración adicional recomendada
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    
    // Opcional: Configuración de usuario
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // ← AGREGAR ESTA LÍNEA

// Configuración de Cloudinary
var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
builder.Services.Configure<CloudinarySettings>(cloudinarySettings); // Asegúrate de tener la clase CloudinarySettings definida

// Agregar Cloudinary como singleton (ya lo tenías)
var account = new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);
builder.Services.AddSingleton(new Cloudinary(account));

// Agregar nuestro servicio CloudinaryService
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>(); // Asegúrate de tener estas interfaces y clases definidas


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Asegúrate de tener esta línea
app.UseRouting();

// IMPORTANTE: Agregar UseAuthentication antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();