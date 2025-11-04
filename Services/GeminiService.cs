// Services/GeminiService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace PROYECTOMOVIE.Services
{
    public class GeminiService : IGeminiService, IDisposable
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public GeminiService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
        }

        public async Task<string> GenerateStoryAsync(string genre, string mainCharacter, string setting, string additionalDetails = "")
        {
            var prompt = $@"
            Eres un escritor creativo profesional. Crea una historia original con las siguientes características:

            GÉNERO: {genre}
            PERSONAJE PRINCIPAL: {mainCharacter}
            ESCENARIO: {setting}
            DETALLES ADICIONALES: {additionalDetails}

            INSTRUCCIONES:
            - Longitud: 300-500 palabras
            - Estructura: introducción, desarrollo y conclusión
            - Incluye diálogos naturales y descripciones vívidas
            - Mantén un tono coherente con el género
            - Desarrolla el personaje principal
            - Crea un conflicto interesante y su resolución

            Responde ÚNICAMENTE con el texto de la historia, sin títulos, comentarios o notas adicionales.
            ";

            return await CallGeminiAPI(prompt);
        }

        public async Task<string> ContinueStoryAsync(string existingStory, string continuationPrompt)
        {
            var prompt = $@"
            HISTORIA EXISTENTE:
            {existingStory}

            CONTINUACIÓN SOLICITADA: {continuationPrompt}

            INSTRUCCIONES:
            - Continúa la historia de manera coherente
            - Mantén el estilo, tono y personajes existentes
            - Agrega aproximadamente 200-300 palabras
            - Desarrolla la trama naturalmente
            - No repitas información ya mencionada

            Responde ÚNICAMENTE con la continuación de la historia.
            ";

            return await CallGeminiAPI(prompt);
        }

        public async Task<string> ImproveStoryAsync(string existingStory, string improvementType)
        {
            var improvementInstruction = improvementType.ToLower() switch
            {
                "diálogos" => "Mejora todos los diálogos haciéndolos más naturales, expresivos y característicos de cada personaje.",
                "descripciones" => "Enriquece las descripciones de lugares, personajes y acciones. Hazlas más vívidas y sensoriales.",
                "estructura" => "Mejora el flujo narrativo, los puntos de giro y el ritmo de la historia.",
                "personajes" => "Profundiza en el desarrollo de los personajes, sus motivaciones y evolución.",
                _ => "Mejora la calidad general de la escritura: vocabulario, sintaxis y estilo literario."
            };

            var prompt = $@"
            HISTORIA ORIGINAL:
            {existingStory}

            TIPO DE MEJORA: {improvementInstruction}

            INSTRUCCIONES:
            - Mantén la trama principal intacta
            - Conserva los personajes y eventos clave
            - Mejora específicamente: {improvementType}
            - No agregues nuevos eventos o personajes principales

            Devuelve la historia completa mejorada.
            ";

            return await CallGeminiAPI(prompt);
        }

        public async Task<string> GenerateStoryTitleAsync(string content)
        {
            var prompt = $@"
            Basándote en el siguiente texto de una historia, genera un título creativo y apropiado.
            El título debe ser máximo 5-7 palabras y capturar la esencia de la historia.

            Historia:
            {content.Substring(0, Math.Min(500, content.Length))}

            Responde SOLO con el título, sin comillas ni puntos finales.
            ";

            return await CallGeminiAPI(prompt);
        }

        private async Task<string> CallGeminiAPI(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.8,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 2048
                    }
                };

                var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseContent);

                // Extraer el texto de la respuesta
                if (jsonDocument.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0)
                {
                    var candidate = candidates[0];
                    if (candidate.TryGetProperty("content", out var contentElement) &&
                        contentElement.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        var text = parts[0].GetProperty("text").GetString();
                        return text?.Trim() ?? "No se pudo generar contenido.";
                    }
                }

                throw new Exception("Respuesta de API en formato inesperado");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al llamar a la API de Gemini: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}