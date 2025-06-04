namespace TelegramBotFramework.Services
{
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class UserJoinedUpdateHandler(IQuestionService questionService, IPendingQuestionsService pendingQuestionsService) : ICanUpdateHandler
    {
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update is { Type: UpdateType.Message, Message.NewChatMembers: not null })
            {
                foreach (var user in update.Message.NewChatMembers)
                {
                    var question = questionService.GetNextQuestion();
                    await pendingQuestionsService.AddQuestion(user, question);
                    await botClient.SendMessage(update.Message.Chat.Id, $"{user.FirstName}: {question.QuestionText}?", cancellationToken: cancellationToken);
                }
            }

            if (update is { Type: UpdateType.Message, Message.From: not null, Message.Text: not null })
            {
                var pendingUsers = await pendingQuestionsService.GetPendingUsers();
                if (pendingUsers.Any(x => x.Id == update.Message.From.Id))
                {
                    await pendingQuestionsService.CheckAnswer(update.Message.From, update.Message.Text);
                }
            }
        }

        public Task<bool> CanHandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
        {
            // We need to handle both messages from new chat members and
            // subsequent answers from users. Returning true for any
            // "Message" update ensures we don't ignore messages that
            // contain potential answers.
            return Task.FromResult(update.Type == UpdateType.Message);
        }
    }
}