// Controllers/StoriesController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PROYECTOMOVIE.Models;
using PROYECTOMOVIE.Services;

namespace PROYECTOMOVIE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private static List<Story> _stories = new();

        public StoriesController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStory([FromBody] CreateStoryRequest request)
        {
            try
            {
                var storyContent = await _geminiService.GenerateStoryAsync(
                    request.Genre,
                    request.MainCharacter,
                    request.Setting,
                    request.AdditionalDetails
                );

                var story = new Story
                {
                    Title = await _geminiService.GenerateStoryTitleAsync(storyContent), // Cambio aquí
                    Content = storyContent,
                    Genre = request.Genre,
                    MainCharacter = request.MainCharacter,
                    Setting = request.Setting
                };

                _stories.Add(story);

                return Ok(new { success = true, story = story });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("{id}/continue")]
        public async Task<IActionResult> ContinueStory(string id, [FromBody] ContinueStoryRequest request)
        {
            try
            {
                var existingStory = _stories.FirstOrDefault(s => s.Id == id);
                if (existingStory == null)
                    return NotFound(new { success = false, error = "Historia no encontrada" });

                var continuation = await _geminiService.ContinueStoryAsync(
                    existingStory.Content,
                    request.Prompt
                );

                existingStory.Content += "\n\n" + continuation;

                return Ok(new { success = true, story = existingStory });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("{id}/improve")]
        public async Task<IActionResult> ImproveStory(string id, [FromBody] ImproveStoryRequest request)
        {
            try
            {
                var existingStory = _stories.FirstOrDefault(s => s.Id == id);
                if (existingStory == null)
                    return NotFound(new { success = false, error = "Historia no encontrada" });

                var improvedContent = await _geminiService.ImproveStoryAsync(
                    existingStory.Content,
                    request.ImprovementType
                );

                existingStory.Content = improvedContent;

                return Ok(new { success = true, story = existingStory });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllStories()
        {
            return Ok(new { success = true, stories = _stories });
        }

        [HttpGet("{id}")]
        public IActionResult GetStory(string id)
        {
            var story = _stories.FirstOrDefault(s => s.Id == id);
            if (story == null)
                return NotFound(new { success = false, error = "Historia no encontrada" });

            return Ok(new { success = true, story = story });
        }

        // Eliminar el método GenerateStoryTitle privado ya que ahora está en el servicio
    }
}