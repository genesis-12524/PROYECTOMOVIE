using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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

// AGREGAR ESTO: Servicio de email dummy para desarrollo
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

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

// AGREGAR ESTA CLASE: Implementación dummy de IEmailSender
public class DummyEmailSender : IEmailSender
{
    private readonly ILogger<DummyEmailSender> _logger;

    public DummyEmailSender(ILogger<DummyEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // En desarrollo, solo logueamos la información en lugar de enviar email
        _logger.LogInformation("=== EMAIL SIMULADO ===");
        _logger.LogInformation($"Para: {email}");
        _logger.LogInformation($"Asunto: {subject}");
        _logger.LogInformation($"Mensaje: {htmlMessage}");
        _logger.LogInformation("=== FIN EMAIL SIMULADO ===");
        
        return Task.CompletedTask;
    }
}