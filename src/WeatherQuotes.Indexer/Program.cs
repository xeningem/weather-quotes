using OpenAI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using WeatherQuotes.Indexer;

const string booksDirectory = "books";
const string collectionName = "literary_quotes";
const ulong vectorSize = 768;
const int batchSize = 50;

var ollamaClient = new OpenAIClient(
    new ApiKeyCredential("ollama"),
    new OpenAIClientOptions { Endpoint = new Uri("http://localhost:11434/v1") }
);

IEmbeddingGenerator<string, Embedding<float>> embeddings =
    ollamaClient.GetEmbeddingClient("nomic-embed-text").AsIEmbeddingGenerator();

var qdrant = new QdrantClient("localhost");

Console.WriteLine("=== Downloading missing books ===");
await BookDownloader.DownloadMissingAsync(booksDirectory);

Console.WriteLine("\n=== Preparing Qdrant collection ===");
var startOffset = await EnsureCollectionAsync(qdrant, collectionName, vectorSize);

Console.WriteLine("\n=== Indexing books ===");
var quotes = GutenbergLoader.LoadBooks(booksDirectory);
Console.WriteLine($"Extracted {quotes.Count} weather-related paragraphs.");

if (startOffset > 0)
    Console.WriteLine($"Resuming from {startOffset}/{quotes.Count} (already indexed).");

var startTime = DateTime.UtcNow;
var processed = 0;

for (int i = (int)startOffset; i < quotes.Count; i += batchSize)
{
    var batch = quotes.Skip(i).Take(batchSize).ToList();
    var vectors = await embeddings.GenerateAsync(batch.Select(b => b.Text).ToList());

    var points = new List<PointStruct>();
    for (int j = 0; j < batch.Count; j++)
    {
        var q = batch[j];
        points.Add(new PointStruct
        {
            Id = new PointId { Num = (ulong)(i + j) },
            Vectors = vectors[j].Vector.ToArray(),
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
    processed += batch.Count;

    var done = i + batch.Count;
    var elapsed = DateTime.UtcNow - startTime;
    var eta = processed > 0
        ? TimeSpan.FromSeconds(elapsed.TotalSeconds / processed * (quotes.Count - done))
        : TimeSpan.Zero;

    Console.WriteLine($"  {done}/{quotes.Count}  elapsed {elapsed:mm\\:ss}  ETA {eta:mm\\:ss}");
}

Console.WriteLine("\nIndexing complete.");

static async Task<ulong> EnsureCollectionAsync(QdrantClient client, string name, ulong size)
{
    var collections = await client.ListCollectionsAsync();
    if (collections.Any(c => c == name))
    {
        var info = await client.GetCollectionInfoAsync(name);
        var count = info.PointsCount;
        if (count > 0)
        {
            Console.WriteLine($"Collection '{name}' exists with {count} points — resuming.");
            return count;
        }
        await client.DeleteCollectionAsync(name);
        Console.WriteLine($"Deleted empty collection '{name}'.");
    }
    await client.CreateCollectionAsync(name, new VectorParams { Size = size, Distance = Distance.Cosine });
    Console.WriteLine($"Collection '{name}' created ({size} dims).");
    return 0;
}
