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

    public async Task<string> GenerateAsync(string prompt)
    {
        var chatClient = Client.GetChatClient("gemma-3-27b");
        var response = await chatClient.CompleteChatAsync([prompt]);
        return response.Value.Content[0].Text;
    }

    public async IAsyncEnumerable<string> GenerateManyAsync(string prompt)
    {
        var chatClient = Client.GetChatClient("gemma-3-27b");
        await foreach (var message in chatClient.CompleteChatStreamingAsync([prompt]))
        {
            if (message.ContentUpdate is not null)
                foreach (var update in message.ContentUpdate)
                    yield return update.Text ?? string.Empty;
        }
    }
}