using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;
using PROYECTOMOVIE.interfaze;


[ApiController]
[Route("api/[controller]")]
public class PeliculasController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ApplicationDbContext _context;

    public PeliculasController(
        ICloudinaryService cloudinaryService,
        ApplicationDbContext context)
    {
        _cloudinaryService = cloudinaryService;
        _context = context;
    }

    [HttpPost("crear-pelicula")]
    public async Task<IActionResult> CrearPelicula([FromForm] PeliculaCreateModel model)
    {
        try
        {
            // 1. Subir IMAGEN de la película
            var imageResult = await _cloudinaryService.UploadImageAsync(model.ImagenFile!, "peliculas-imagenes");
            if (imageResult.Error != null)
                return BadRequest($"Error subiendo imagen: {imageResult.Error.Message}");

            // 2. Subir TRAILER
            var trailerResult = await _cloudinaryService.UploadVideoAsync(model.TrailerFile!, "peliculas-trailers", true);
            if (trailerResult.Error != null)
            {
                await _cloudinaryService.DeleteResourceAsync(imageResult.PublicId);
                return BadRequest($"Error subiendo trailer: {trailerResult.Error.Message}");
            }

            // 3. Subir PELÍCULA COMPLETA
            var movieResult = await _cloudinaryService.UploadVideoAsync(model.PeliculaFile!, "peliculas-completas", false);
            if (movieResult.Error != null)
            {
                await _cloudinaryService.DeleteResourceAsync(imageResult.PublicId);
                await _cloudinaryService.DeleteResourceAsync(trailerResult.PublicId, "video");
                return BadRequest($"Error subiendo película: {movieResult.Error.Message}");
            }

            // Guardar en base de datos
            var pelicula = new Pelicula
            {
                Nombre_Peli = model.Nombre_Peli,
                Descripción = model.Descripción,
                Tiempo_Duracion = model.Tiempo_Duracion,
                Fecha_Publicada = model.Fecha_Publicada,
                // IMAGEN
                Imagen_Peli = imageResult.SecureUrl.ToString(),
                ImagenPublicId = imageResult.PublicId,
                // TRAILER
                Video_Trailer = trailerResult.SecureUrl.ToString(),
                TrailerPublicId = trailerResult.PublicId,
                // PELÍCULA COMPLETA
                Enlace_Peli = movieResult.SecureUrl.ToString(),
                PeliculaPublicId = movieResult.PublicId
            };

            _context.Peliculas.Add(pelicula);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = pelicula.Id,
                Nombre_Peli = pelicula.Nombre_Peli,
                Imagen_Peli = pelicula.Imagen_Peli,
                Video_Trailer = pelicula.Video_Trailer,
                Enlace_Peli = pelicula.Enlace_Peli,
                Fecha_Publicada = pelicula.Fecha_Publicada
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("todas-peliculas")]
    public async Task<IActionResult> GetTodasPeliculas()
    {
        var peliculas = await _context.Peliculas
            .OrderByDescending(p => p.Fecha_Publicada)
            .Select(p => new
            {
                p.Id,
                p.Nombre_Peli,
                p.Descripción,
                p.Imagen_Peli,
                p.Video_Trailer,
                p.Enlace_Peli,
                p.Tiempo_Duracion,
                p.Fecha_Publicada
            })
            .ToListAsync();

        return Ok(peliculas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPelicula(int id)
    {
        var pelicula = await _context.Peliculas
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.Nombre_Peli,
                p.Descripción,
                p.Imagen_Peli,
                p.Video_Trailer,
                p.Enlace_Peli,
                p.Tiempo_Duracion,
                p.Fecha_Publicada
            })
            .FirstOrDefaultAsync();

        if (pelicula == null) return NotFound();

        return Ok(pelicula);
    }

    [HttpDelete("eliminar/{id}")]
    public async Task<IActionResult> EliminarPelicula(int id)
    {
        var pelicula = await _context.Peliculas
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pelicula == null) return NotFound();

        try
        {
            // Eliminar todos los recursos de Cloudinary
            if (!string.IsNullOrEmpty(pelicula.ImagenPublicId))
                await _cloudinaryService.DeleteResourceAsync(pelicula.ImagenPublicId);

            if (!string.IsNullOrEmpty(pelicula.TrailerPublicId))
                await _cloudinaryService.DeleteResourceAsync(pelicula.TrailerPublicId, "video");

            if (!string.IsNullOrEmpty(pelicula.PeliculaPublicId))
                await _cloudinaryService.DeleteResourceAsync(pelicula.PeliculaPublicId, "video");

            // Eliminar de la base de datos
            _context.Peliculas.Remove(pelicula);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Película eliminada exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error eliminando película: {ex.Message}");
        }
    }
}