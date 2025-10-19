using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using PROYECTOMOVIE.Models.config;

namespace PROYECTOMOVIE.interfaze
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder = "peliculas-imagenes")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("El archivo está vacío");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Formato de imagen no válido");

            await using var stream = file.OpenReadStream();
            
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation().Width(800).Height(1200).Crop("fill")
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

                public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file, string folder = "peliculas-videos", bool isTrailer = false)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("El archivo está vacío");

            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".wmv", ".flv", ".mkv", ".webm" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Formato de video no válido");

            // Límites diferentes para trailer vs película completa
            long maxSize = isTrailer ? 50 * 1024 * 1024 : 500 * 1024 * 1024; // 50MB para trailer, 500MB para película
            if (file.Length > maxSize)
                throw new ArgumentException($"El tamaño del archivo excede el límite de {(isTrailer ? "50MB" : "500MB")}");

            await using var stream = file.OpenReadStream();
            
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                // Quitamos la línea ResourceType porque es de solo lectura y ya es "video" por defecto.
                Transformation = isTrailer 
                    ? new Transformation().Width(1280).Height(720).Crop("limit")
                    : new Transformation().Width(1920).Height(1080).Crop("limit")
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        public async Task<DeletionResult> DeleteResourceAsync(string publicId, string resourceType = "image")
        {
            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = resourceType == "video" ? ResourceType.Video : ResourceType.Image
            };
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}