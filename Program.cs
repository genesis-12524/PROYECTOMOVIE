using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using CloudinaryDotNet;
using PROYECTOMOVIE.interfaze;
using PROYECTOMOVIE.Models.config;
using PROYECTOMOVIE.Services;

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
.AddDefaultTokenProviders();

// ✅ AGREGAR CONFIGURACIÓN DE SESSION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".PROYECTOMOVIE.Session";
});

// Configuración de Cloudinary
var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
builder.Services.Configure<CloudinarySettings>(cloudinarySettings);

// Agregar Cloudinary como singleton
var account = new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);
builder.Services.AddSingleton(new Cloudinary(account));

// Agregar nuestro servicio CloudinaryService
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Configurar Gemini Service
var geminiApiKey = builder.Configuration["Gemini:ApiKey"] 
    ?? throw new ArgumentNullException("Gemini:ApiKey no configurado");

builder.Services.AddSingleton<IGeminiService>(provider => 
    new GeminiService(geminiApiKey));

// CORS para el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// MVC y API Services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// IMPORTANTE: Agregar UseAuthentication antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseCors("AllowAll");
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

// Configuración adicional para APIs
app.UseDefaultFiles();
app.MapControllers();

app.Run();