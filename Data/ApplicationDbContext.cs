using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Models;
using Microsoft.AspNetCore.Identity;

namespace PROYECTOMOVIE.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PlanSubcripcion> PlanSubcripciones { get; set; }
        public DbSet<UsuarioSuscripcion> UsuarioSuscripciones { get; set; }
        public DbSet<UsuarioPelicula> UsuarioPeliculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. CONFIGURACIÓN DE PELICULA
            builder.Entity<Pelicula>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombre_Peli).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Imagen_Peli).IsRequired();
                entity.Property(p => p.Descripción).IsRequired();
                entity.Property(p => p.Genero).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Enlace_Peli).IsRequired();
                entity.Property(p => p.Video_Trailer).IsRequired();
                entity.Property(p => p.Tiempo_Duracion).IsRequired();
                entity.Property(p => p.Fecha_Publicada).IsRequired();
            });

            // 2. CAMBIAR NOMBRE DE TABLA DE USUARIOS
            builder.Entity<Usuario>().ToTable("t_usuario");

            // 3. CONFIGURACIÓN USUARIO PELICULA (Relación Many-to-Many)
            builder.Entity<UsuarioPelicula>(entity =>
            {
                // Clave primaria compuesta
                entity.HasKey(up => new { up.UsuarioId, up.PeliculaId });

                // Relación con Usuario
                entity.HasOne(up => up.Usuario)
                    .WithMany(u => u.UsuarioPeliculas)
                    .HasForeignKey(up => up.UsuarioId);

                // Relación con Pelicula
                entity.HasOne(up => up.Pelicula)
                    .WithMany(p => p.UsuarioPeliculas)
                    .HasForeignKey(up => up.PeliculaId);
            });


            // SEED DATA PARA para Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "2c040acd-d2fb-43ef-5fc5-c2e3f886ff01",
                    Name = "Cliente",
                    NormalizedName = "CLIENTE",
                    ConcurrencyStamp = "null"
                },
                new IdentityRole
                {
                    Id = "3a8e1fdb-7c2d-4a5e-8f1c-9d3b2a1edf5c",
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR",
                    ConcurrencyStamp = "null"
                }                 
            );
             

            // 4. CONFIGURACIÓN SUSCRIPCIONES 

            // Relación Usuario - UsuarioSuscripcion (1:1)
            builder.Entity<Usuario>()
                .HasOne(u => u.Subcripcion)
                .WithOne(us => us.Usuario)
                .HasForeignKey<UsuarioSuscripcion>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación PlanSubcripcion - UsuarioSuscripcion (1:N)
            builder.Entity<PlanSubcripcion>()
                .HasMany(p => p.UsuarioSuscripcion)
                .WithOne(us => us.Plan)
                .HasForeignKey(us => us.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar PlanSubcripcion
            builder.Entity<PlanSubcripcion>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Precio).HasColumnType("decimal(10,2)");
                entity.Property(p => p.Descripcion).HasMaxLength(500);
            });

            // Configurar UsuarioSuscripcion
            builder.Entity<UsuarioSuscripcion>(entity =>
            {
                entity.HasKey(us => us.Id);
                entity.Property(us => us.MercadoPagoSubscriptionId).HasMaxLength(100);
                entity.Property(us => us.MercadoPagoPayerId).HasMaxLength(100);
                entity.Property(us => us.MercadoPagoCardId).HasMaxLength(100);
            });

            // 5. ÍNDICES PARA MEJOR PERFORMANCE
            builder.Entity<UsuarioSuscripcion>()
                .HasIndex(us => us.UserId)
                .IsUnique(); // Para relación 1:1

            builder.Entity<UsuarioSuscripcion>()
                .HasIndex(us => new { us.Status, us.NextBillingDate });


            // 6. SEED DATA PARA PLANES (DATOS INICIALES)
            builder.Entity<PlanSubcripcion>().HasData(
                new PlanSubcripcion 
                { 
                    Id = 1,
                    Nombre = "Plan Básico", 
                    Precio = 14.90m,
                    Descripcion = "Ideal para un espectador - 1 pantalla, calidad HD",
                    CicloFacturacion = 30,
                    Activo = true
                },
                new PlanSubcripcion 
                { 
                    Id = 2,
                    Nombre = "Plan Estándar", 
                    Precio = 24.90m,
                    Descripcion = "Perfecto para familias - 3 pantallas, Full HD", 
                    CicloFacturacion = 30,
                    Activo = true
                },
                new PlanSubcripcion 
                { 
                    Id = 3,
                    Nombre = "Plan Premium", 
                    Precio = 34.90m,
                    Descripcion = "Experiencia cinematográfica - 5 pantallas, calidad 4K",
                    CicloFacturacion = 30,
                    Activo = true
                }
            );
        }
    }
}