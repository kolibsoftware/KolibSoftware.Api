using System.ClientModel;
using System.Text;
using OllamaSharp;
using OllamaSharp.Models;
using OpenAI;

namespace KolibSoftware.Api.Example.Services;

public class BitNetService(string host = "http://server.lan:8080")
{
    public OpenAIClient Client { get; } = new OpenAIClient(new ApiKeyCredential("KEY"), new()
    {
        Endpoint = new Uri(host)
    });

    public async Task<string> GenerateAsync(string prompt)
    {
        var sb = new StringBuilder();
        await foreach (var message in GenerateManyAsync(prompt))
            sb.Append(message);
        return sb.ToString();
    }

    public async IAsyncEnumerable<string> GenerateManyAsync(string prompt)
    {
        var chatClient = Client.GetChatClient("bitnet");
        await foreach (var message in chatClient.CompleteChatStreamingAsync([prompt]))
        {
            if (message.ContentUpdate is not null)
                foreach (var update in message.ContentUpdate)
                    yield return update.Text;
        }
    }
}