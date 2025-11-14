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
        // Le dice a Entity Framework que "Categoria" es una tabla en tu base de datos.
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Serie> Series { get; set; }

        public DbSet<Plan> Planes { get; set; }
        public DbSet<Subscripcion> Subscripciones { get; set; }
        public DbSet<UsuarioSerie> UsuarioSeries { get; set; }
        public DbSet<UsuarioPelicula> UsuarioPeliculas { get; set; }

        public DbSet<VideoConfigPortada> VideoConfigPortadas { get; set; }

        public DbSet<Lista> DataLista { get; set; }
        public DbSet<ListaPelicula> DataListaPelicula { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {

            // Configuración de la llave primaria compuesta para UsuarioSerie
            builder.Entity<UsuarioSerie>()
            .HasKey(us => new { us.UsuarioId, us.SerieId });
            
            // **IMPORTANTE:** Haz lo mismo para UsuarioPelicula si aún no lo has hecho
            builder.Entity<UsuarioPelicula>()
            .HasKey(up => new { up.UsuarioId, up.PeliculaId });



            // 2. CONFIGURACIÓN DEL SEEDING para Categorias (Para que los datos se queden)
            builder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Acción" },
                new Categoria { Id = 2, Nombre = "Comedia" },
                new Categoria { Id = 3, Nombre = "Drama" },
                new Categoria { Id = 4, Nombre = "Ciencia Ficción" },
                new Categoria { Id = 5, Nombre = "Terror" }
            );

            // 3. Configuración de la relación M:M entre Pelicula y Categoria (sin tabla de unión explícita)
            // Esto creará la tabla de unión 'PeliculaCategoria' por ti.
            builder.Entity<Pelicula>()
                .HasMany(p => p.Categorias) // Asumiendo que agregaste ICollection<Categoria> a Pelicula
                .WithMany(c => c.Peliculas);

            // 4. Configuración de la relación M:M entre Serie y Categoria
            // Esto creará la tabla de unión 'SerieCategoria' por ti.
            builder.Entity<Serie>()
                .HasMany(s => s.Categorias) // Asumiendo que agregaste ICollection<Categoria> a Serie
                .WithMany(c => c.Series);

            base.OnModelCreating(builder);
            
            // CONFIGURACIÓN DE RELACIONES para Subscripcion
            builder.Entity<Subscripcion>(entity =>
            {
                entity.HasKey(s => s.Id);
                
                entity.HasOne(s => s.User)
                    .WithMany(u => u.Subscripciones)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(s => s.Plan)
                    .WithMany()
                    .HasForeignKey(s => s.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 1. CONFIGURACIÓN DE PELICULA
            builder.Entity<Pelicula>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombre_Peli).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Imagen_Peli).IsRequired();
                entity.Property(p => p.Descripción).IsRequired();
                entity.Property(p => p.Enlace_Peli).IsRequired();
                entity.Property(p => p.Video_Trailer).IsRequired();
                entity.Property(p => p.Tiempo_Duracion).IsRequired();
                entity.Property(p => p.Fecha_Publicada).IsRequired();

                entity.HasMany(p => p.Categorias)
                .WithMany(c => c.Peliculas)
                .UsingEntity("CategoriaPelicula"); // Nombre de la tabla intermedia
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
             


            // ========================================
            // CONFIGURACIÓN DE LISTAS PERSONALIZADAS (NUEVO)
            // ========================================
    
            builder.Entity<Lista>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Descripcion).HasMaxLength(500);
                entity.Property(l => l.UsuarioId).IsRequired();
        
                entity.HasOne(l => l.Usuario)
                    .WithMany()
                    .HasForeignKey(l => l.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ListaPelicula>(entity =>
            {
                entity.HasKey(lp => new { lp.ListaId, lp.PeliculaId });
        
                entity.HasOne(lp => lp.Lista)
                    .WithMany(l => l.ListaPeliculas)
                    .HasForeignKey(lp => lp.ListaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(lp => lp.Pelicula)
                    .WithMany()
                    .HasForeignKey(lp => lp.PeliculaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Plan>().HasData(
                new Plan
                {
                    Id = 1,
                    Nombre = "Básico", 
                    Descripcion = "1 pantalla - Calidad HD", 
                    Precio = 19.90m, 
                    MercadoPagoPlanId = "b8c80fe330ab4dcb8ab6bf29596c8e05",
                    DuracionDias = 30,
                    Activo = true
                },
                new Plan
                {
                    Id = 2,
                    Nombre = "Premium", 
                    Descripcion = "4 pantallas - Calidad 4K", 
                    Precio = 29.90m, 
                    MercadoPagoPlanId = "6f044d2acc954b0798c8440c384945df",
                    DuracionDias = 30,
                    Activo = true
                }
            );

        }
    }
}