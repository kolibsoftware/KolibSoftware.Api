using System.ClientModel;
using System.Text;
using OpenAI;

namespace KolibSoftware.Api.Example.Services;

public class LocalAiService(string host = "http://server.lan:8081")
{
    public OpenAIClient Client { get; } = new OpenAIClient(new ApiKeyCredential("KEY"), new()
    {
        Endpoint = new Uri(host)
    });

    public async Task<string> GenerateAsync(string prompt)
    {
        var chatClient = Client.GetChatClient("nvidia_llama-3.1-nemotron-nano-4b-v1.1");
        var response = await chatClient.CompleteChatAsync([prompt]);
        return response.Value.Content[0].Text;
    }

    public async IAsyncEnumerable<string> GenerateManyAsync(string prompt)
    {
        var chatClient = Client.GetChatClient("nvidia_llama-3.1-nemotron-nano-4b-v1.1");
        await foreach (var message in chatClient.CompleteChatStreamingAsync([prompt]))
        {
            if (message.ContentUpdate is not null)
                foreach (var update in message.ContentUpdate)
                    yield return update.Text ?? string.Empty;
        }
    }
}