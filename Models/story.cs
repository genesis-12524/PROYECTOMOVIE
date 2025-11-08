using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Models/Story.cs
namespace PROYECTOMOVIE.Models
{
    public class Story
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string MainCharacter { get; set; } = string.Empty;
        public string Setting { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int WordCount => Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    // DTOs para las requests
    public class CreateStoryRequest
    {
        public string Genre { get; set; } = string.Empty;
        public string MainCharacter { get; set; } = string.Empty;
        public string Setting { get; set; } = string.Empty;
        public string AdditionalDetails { get; set; } = string.Empty;
    }

    public class ContinueStoryRequest
    {
        public string Prompt { get; set; } = string.Empty;
    }

    public class ImproveStoryRequest
    {
        public string ImprovementType { get; set; } = "general";
    }
}