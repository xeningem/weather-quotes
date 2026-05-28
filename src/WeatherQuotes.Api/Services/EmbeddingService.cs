using Microsoft.Extensions.AI;

namespace WeatherQuotes.Api.Services;

public class EmbeddingService(IEmbeddingGenerator<string, Embedding<float>> embeddings)
{
    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var results = await embeddings.GenerateAsync([text]);
        return results[0].Vector.ToArray();
    }
}
