namespace TelegramBotFramework.Services
{
    using Telegram.Bot.Types;

    public class PendingQuestionsService : IPendingQuestionsService
    {
        public Task AddQuestion(User user, Question question) =>
            throw new NotImplementedException();

        public Task CheckAnswer(User user, string answer) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<User>> GetPendingUsers() =>
            throw new NotImplementedException();
    }
}