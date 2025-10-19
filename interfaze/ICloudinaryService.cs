using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace PROYECTOMOVIE.interfaze
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder = "peliculas-imagenes");
        Task<VideoUploadResult> UploadVideoAsync(IFormFile file, string folder = "peliculas-videos", bool isTrailer = false);
        Task<DeletionResult> DeleteResourceAsync(string publicId, string resourceType = "image");
    }
}