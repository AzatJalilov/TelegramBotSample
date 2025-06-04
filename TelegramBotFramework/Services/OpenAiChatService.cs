namespace TelegramBotFramework.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using ModelContextProtocol.Client;
    using System.ClientModel;
    using System.ClientModel.Primitives;
    using OpenAI.Chat;

    public class OpenAiChatService : IOpenAiChatService
    {
        private readonly ChatClient _client;
        private readonly IMcpClient _mcpClient;

        public OpenAiChatService(string authToken, IMcpClient mcpClient)
            : this(new ChatClient(model: "gpt-4o", apiKey: authToken), mcpClient)
        {
        }

        public OpenAiChatService(ChatClient client, IMcpClient mcpClient)
        {
            _client = client;
            _mcpClient = mcpClient;
        }

        public async Task<string> GetCompletion(string userText, CancellationToken cancellationToken = default)
        {
            var messages = new ChatMessage[]
            {
                new SystemChatMessage("You are a helpful assistant."),
                new UserChatMessage(userText)
            };

            IList<McpClientTool> tools = await _mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

            ChatCompletionOptions options = new();
            foreach (var tool in tools)
            {
                var schemaJson = tool.ProtocolTool.InputSchema.GetRawText();
                ChatTool chatTool = ChatTool.CreateFunctionTool(
                    functionName: tool.Name,
                    functionDescription: tool.Description,
                    functionParameters: BinaryData.FromString(schemaJson));
                options.Tools.Add(chatTool);
            }

            ClientResult<ChatCompletion> completion = await _client.CompleteChatAsync(messages, options, cancellationToken);
            ChatCompletion result = completion.Value;
            return result.Content.Count > 0 ? result.Content[0].Text : string.Empty;
        }
    }
}
