using System.ClientModel;
using OpenAI;

namespace KolibSoftware.Api.Example.Services;

public class OpenAIService(
    IConfiguration configuration,
    string host = "http://10.8.0.253:8080/v1")
{
    public OpenAIClient Client { get; } = new OpenAIClient(new ApiKeyCredential(configuration["OpenAI:ApiKey"]!), new()
    {
        Endpoint = new Uri(host)
    });

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var chatClient = Client.GetChatClient("gemma-3-27b");
        var response = await chatClient.CompleteChatAsync([prompt], cancellationToken: cancellationToken);
        return response.Value.Content[0].Text;
    }

    public async Task<float[]> EmbedAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var embeddingClient = Client.GetEmbeddingClient("qwen3-embedding-4b");
        var response = await embeddingClient.GenerateEmbeddingAsync(prompt, cancellationToken: cancellationToken);
        return response.Value.ToFloats().ToArray();
    }
}