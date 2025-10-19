using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Models;

namespace PROYECTOMOVIE.Data;

public class ApplicationDbContext : IdentityDbContext<Usuario>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
 public DbSet<Pelicula> Peliculas { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
        // Cambiar el nombre de la tabla de usuarios
        builder.Entity<Usuario>().ToTable("t_usuario");

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

    }
    
        
    public DbSet<UsuarioPelicula> DSUsuarioPelicula { get; set; }
    
}
