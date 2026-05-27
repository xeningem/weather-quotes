using OpenAI;
using System.ClientModel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using WeatherQuotes.Indexer;

const string booksDirectory = "books";
const string collectionName = "literary_quotes";
const ulong vectorSize = 768;

var ollamaClient = new OpenAIClient(
    new ApiKeyCredential("ollama"),
    new OpenAIClientOptions { Endpoint = new Uri("http://localhost:11434/v1") }
);

var kernel = Kernel.CreateBuilder()
    .AddOpenAITextEmbeddingGeneration("nomic-embed-text", ollamaClient)
    .Build();

var embeddings = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
var qdrant = new QdrantClient("localhost");

Console.WriteLine("=== Downloading missing books ===");
await BookDownloader.DownloadMissingAsync(booksDirectory);

Console.WriteLine("\n=== Preparing Qdrant collection ===");
await RecreateCollectionAsync(qdrant, collectionName, vectorSize);

Console.WriteLine("\n=== Indexing books ===");
var quotes = GutenbergLoader.LoadBooks(booksDirectory);
Console.WriteLine($"Extracted {quotes.Count} weather-related paragraphs.");

const int batchSize = 50;
var points = new List<PointStruct>();
ulong id = 0;

for (int i = 0; i < quotes.Count; i += batchSize)
{
    var batch = quotes.Skip(i).Take(batchSize).ToList();
    var vectors = await embeddings.GenerateEmbeddingsAsync(batch.Select(b => b.Text).ToList());

    for (int j = 0; j < batch.Count; j++)
    {
        var q = batch[j];
        points.Add(new PointStruct
        {
            Id = new PointId { Num = id++ },
            Vectors = vectors[j].ToArray(),
            Payload = 
            { 
                ["text"] = q.Text, 
                ["book"] = q.Book, 
                ["author"] = q.Author,
                ["era"] = q.Metadata.Era,
                ["genre"] = q.Metadata.Genre,
                ["language"] = q.Metadata.Language
            }
        });
    }

    await qdrant.UpsertAsync(collectionName, points);
    points.Clear();
    Console.WriteLine($"  {Math.Min(i + batchSize, quotes.Count)}/{quotes.Count}");
}

Console.WriteLine("\nIndexing complete.");

static async Task RecreateCollectionAsync(QdrantClient client, string name, ulong size)
{
    var collections = await client.ListCollectionsAsync();
    if (collections.Any(c => c == name))
    {
        await client.DeleteCollectionAsync(name);
        Console.WriteLine($"Deleted old collection '{name}'.");
    }
    await client.CreateCollectionAsync(name, new VectorParams { Size = size, Distance = Distance.Cosine });
    Console.WriteLine($"Collection '{name}' created ({size} dims).");
}
