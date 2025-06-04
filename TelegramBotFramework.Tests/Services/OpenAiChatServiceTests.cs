using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.ClientModel.Primitives;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using OpenAI.Chat;
using System.ClientModel;
using TelegramBotFramework.Services;
using Xunit;

namespace TelegramBotFramework.Tests.Services
{
    public class OpenAiChatServiceTests
    {
        private class FakePipelineResponse : PipelineResponse
        {
            private readonly BinaryData _content;
            public FakePipelineResponse(string json) => _content = BinaryData.FromString(json);
            public override int Status => 200;
            public override string ReasonPhrase => "OK";
            protected override PipelineResponseHeaders HeadersCore { get; } = new EmptyHeaders();
            public override Stream? ContentStream { get; set; }
            public override BinaryData Content => _content;
            public override BinaryData BufferContent(CancellationToken cancellationToken = default) => _content;
            public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default) => new(_content);
            public override void Dispose() { }
            private sealed class EmptyHeaders : PipelineResponseHeaders
            {
                public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();
                public override bool TryGetValue(string name, out string? value) { value = null; return false; }
                public override bool TryGetValues(string name, out IEnumerable<string>? values) { values = null; return false; }
            }
        }

        private static ClientResult<ChatCompletion> CreateResult(string text)
        {
            string json = $"{{\"id\":\"1\",\"choices\":[{{\"message\":{{\"role\":\"assistant\",\"content\":\"{text}\"}},\"finish_reason\":\"stop\"}}],\"created\":0,\"model\":\"gpt-4o\"}}";
            var resp = new FakePipelineResponse(json);
            var fromResponse = typeof(ChatCompletion).GetMethod("FromResponse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
            var completion = (ChatCompletion)fromResponse.Invoke(null, new object[] { resp })!;
            var fromValue = typeof(ClientResult).GetMethod("FromValue")!.MakeGenericMethod(typeof(ChatCompletion));
            return (ClientResult<ChatCompletion>)fromValue.Invoke(null, new object[] { completion, resp })!;
        }

        private class FakeChatClient : ChatClient
        {
            private readonly ClientResult<ChatCompletion> _result;
            public FakeChatClient(ClientResult<ChatCompletion> result) : base("gpt-4o", "fake") => _result = result;
            public override Task<ClientResult<ChatCompletion>> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions options = null, CancellationToken cancellationToken = default) => Task.FromResult(_result);
        }

        private class FakeMcpClient : IMcpClient
        {
            public ServerCapabilities ServerCapabilities { get; } = new();
            public Implementation ServerInfo { get; } = new() { Name = "test", Version = "1.0" };
            public string? ServerInstructions => string.Empty;
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
            public Task SendMessageAsync(JsonRpcMessage message, CancellationToken cancellationToken = default) => Task.CompletedTask;
            public IAsyncDisposable RegisterNotificationHandler(string method, Func<JsonRpcNotification, CancellationToken, ValueTask> handler) => new Dummy();
            private class Dummy : IAsyncDisposable { public ValueTask DisposeAsync() => ValueTask.CompletedTask; }
            public Task<JsonRpcResponse> SendRequestAsync(JsonRpcRequest request, CancellationToken cancellationToken = default)
            {
                var tool = new Tool
                {
                    Name = "echo",
                    Description = "Echo",
                    InputSchema = JsonSerializer.Deserialize<JsonElement>("{\"type\":\"object\",\"properties\":{\"message\":{\"type\":\"string\"}},\"required\":[\"message\"]}")
                };
                var result = new ListToolsResult { Tools = new List<Tool> { tool } };
                var json = JsonSerializer.Serialize(result);
                var node = JsonNode.Parse(json);
                return Task.FromResult(new JsonRpcResponse { Id = request.Id, Result = node });
            }
        }

        [Fact]
        public async Task GetCompletion_ReturnsCompletionText()
        {
            var chatResult = CreateResult("pong");
            var service = new OpenAiChatService(new FakeChatClient(chatResult), new FakeMcpClient());
            var result = await service.GetCompletion("ping");
            Assert.Equal("pong", result);
        }
    }
}
