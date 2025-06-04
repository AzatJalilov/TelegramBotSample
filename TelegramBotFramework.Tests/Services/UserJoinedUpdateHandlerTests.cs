using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Services;
using Xunit;

namespace TelegramBotFramework.Tests.Services;

public class UserJoinedUpdateHandlerTests
{
    private class FakeTelegramBotClient : ITelegramBotClient
    {
        public List<(ChatId ChatId, string Text)> SentMessages { get; } = new();
        public bool LocalBotServer => false;
        public long BotId => 1;
        public TimeSpan Timeout { get; set; }
        public IExceptionParser ExceptionsParser { get; set; } = null!;
        public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest { add { } remove { } }
        public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived { add { } remove { } }
        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is Telegram.Bot.Requests.SendMessageRequest send)
            {
                SentMessages.Add((send.ChatId, send.Text));
                var chat = new Chat { Id = send.ChatId.Identifier ?? 0 };
                object result = new Message { Chat = chat };
                return Task.FromResult((TResponse)result);
            }
            throw new NotImplementedException();
        }
        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<bool> TestApi(CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    [Fact]
    public async Task HandleUpdateAsync_SendsQuestionToNewUser()
    {
        var questionService = new SimpleMathQuestionService();
        var pending = new PendingQuestionsService();
        var handler = new UserJoinedUpdateHandler(questionService, pending);
        var bot = new FakeTelegramBotClient();
        var newUser = new User { Id = 123, FirstName = "Test" };
        var update = new Update
        {
            Id = 1,
            Message = new Message
            {
                NewChatMembers = new[] { newUser },
                Chat = new Chat { Id = 2 }
            }
        };

        await handler.HandleUpdateAsync(bot, update, CancellationToken.None);
        Assert.Single(bot.SentMessages);
        Assert.Contains(newUser.FirstName, bot.SentMessages[0].Text);
    }

    [Fact]
    public async Task HandleUpdateAsync_ChecksAnswerForPendingUser()
    {
        var question = new Question { QuestionText = "1+1", Answer = "2" };
        var questionService = new TestQuestionService(question);
        var pending = new PendingQuestionsService();
        var handler = new UserJoinedUpdateHandler(questionService, pending);
        var bot = new FakeTelegramBotClient();
        var user = new User { Id = 5, FirstName = "Test" };
        await pending.AddQuestion(user, question);
        var update = new Update
        {
            Id = 2,
            Message = new Message
            {
                From = user,
                Text = "2",
                Chat = new Chat { Id = 2 }
            }
        };

        await handler.HandleUpdateAsync(bot, update, CancellationToken.None);
        var pendingUsers = await pending.GetPendingUsers();
        Assert.Empty(pendingUsers);
    }

    private class TestQuestionService : IQuestionService
    {
        private readonly Question _question;
        public TestQuestionService(Question question) => _question = question;
        public Question GetNextQuestion() => _question;
    }
}
