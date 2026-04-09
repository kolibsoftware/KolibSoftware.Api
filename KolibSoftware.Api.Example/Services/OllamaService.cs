using System.Text;
using OllamaSharp;
using OllamaSharp.Models;

namespace KolibSoftware.Api.Example.Services;

public class OllamaService(string host = "http://server.lan:11434")
{
    public OllamaApiClient Client { get; } = new OllamaApiClient(host);

    public async Task<float[]> EmbedAsync(string text)
    {
        var embeddings = await EmbedManyAsync([text]);
        return embeddings.FirstOrDefault() ?? [];
    }

    public async Task<IEnumerable<float[]>> EmbedManyAsync(IEnumerable<string> text)
    {
        var response = await Client.EmbedAsync(new EmbedRequest
        {
            Model = "qwen3-embedding:4b-q4_K_M",
            Input = [.. text]
        });
        return response.Embeddings;
    }

    public async Task<string> SummaryAsync(string prompt)
    {
        var sb = new StringBuilder();
        await foreach (var message in SummaryManyAsync(prompt))
            sb.Append(message);
        return sb.ToString();
    }

    public async IAsyncEnumerable<string> SummaryManyAsync(string prompt)
    {
        var generation = Client.GenerateAsync(new GenerateRequest
        {
            Model = "qwen2.5:3b-instruct-q4_K_M",
            Prompt = prompt,
            Think = false,
        });
        await foreach (var message in generation)
        {
            if (message is not null)
                yield return message.Response;
        }
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var sb = new StringBuilder();
        await foreach (var message in GenerateManyAsync(prompt))
            sb.Append(message);
        return sb.ToString();
    }

    public async IAsyncEnumerable<string> GenerateManyAsync(string prompt)
    {
        var generation = Client.GenerateAsync(new GenerateRequest
        {
            Model = "nemotron-3-nano:4b",
            Prompt = prompt,
            Think = true,
        });
        await foreach (var message in generation)
        {
            if (message is not null)
                yield return message.Response;
        }
    }
}