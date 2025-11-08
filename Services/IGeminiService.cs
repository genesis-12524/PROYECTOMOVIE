// Services/IGeminiService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace PROYECTOMOVIE.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateStoryAsync(string genre, string mainCharacter, string setting, string additionalDetails = "");
        Task<string> ContinueStoryAsync(string existingStory, string continuationPrompt);
        Task<string> ImproveStoryAsync(string existingStory, string improvementType);
        Task<string> GenerateStoryTitleAsync(string content); // Nuevo método añadido
    }
}