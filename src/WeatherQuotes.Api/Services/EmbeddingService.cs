using Microsoft.SemanticKernel.Embeddings;

namespace WeatherQuotes.Api.Services;

public class EmbeddingService(ITextEmbeddingGenerationService embeddings)
{
    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var result = await embeddings.GenerateEmbeddingAsync(text);
        return result.ToArray();
    }
}
